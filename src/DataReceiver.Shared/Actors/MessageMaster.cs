using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Persistence;
using DataReceiver.Shared.Messages;
using MessagePublisher.Shared.Actors;
using MessagePublisher.Shared.Config;
using MessagePublisher.Shared.Messages;

namespace DataReceiver.Shared.Actors
{
    public class MessageMaster : AtLeastOnceDeliveryReceiveActor
    {
        public override string PersistenceId => Context.Self.Path.Name;
        private IActorRef _messageReceiver;
        private MessageReceiverConfig _config;
        private Dictionary<PublisherMessageMeta, long> _externalIDLookup;
        private ICancelable _recurringSnapshotCleanup;
        private IActorRef _targetActor;

        public MessageMaster(MessageReceiverConfig config, IActorRef router)
        {
            _targetActor = router;
            _config = config;
            _externalIDLookup = new Dictionary<PublisherMessageMeta, long>();
            Become(ReceiveMessage);
        }

        private void ReceiveMessage()
        {
            Command<IEnumerable<IPublisherMessage>>(messages =>
            {
                foreach (var message in messages)
                {
                    PublisherMessageMeta header = new PublisherMessageMeta(message.SeqNumber, message.Queue);
                    Deliver(_targetActor.Path, messageId =>
                    {
                        _externalIDLookup[header] = messageId;
                        return message;
                    });
                }
                Sender.Tell(StreamAck.Instance);
            });

            Command<ReliableDeliveryAck<PublisherMessageMeta>>(ack =>
            {        
                long internalMessageID;
                if (_externalIDLookup.TryGetValue(ack.MessageMeta, out internalMessageID))
                {
                    ConfirmDelivery(internalMessageID);
                    _externalIDLookup.Remove(ack.MessageMeta);
                }
            });

            Command<ClearSnapshots>(_ =>
            {
                var snapshot = GetDeliverySnapshot().UnconfirmedDeliveries;
                List<long> settledKeys = new List<long>();
                foreach (var delivery in snapshot)
                {
                    var targetPath = delivery.Destination.Name;
                    IPublisherMessage externalMessage = (IPublisherMessage)delivery.Message;
                    var header = new PublisherMessageMeta(externalMessage.SeqNumber, externalMessage.Queue);
                    if (!_externalIDLookup.ContainsKey(header))
                    {
                        ConfirmDelivery(delivery.DeliveryId);
                    }
                }
            });

            Command<StreamInit>(_ =>
            {
                Sender.Tell(StreamAck.Instance);
            });
        }

        protected override void PreStart()
        {
            _messageReceiver = Context.ActorOf(Props.Create(() => new MessageReceiver(Self,
                    _config)), "message-receiver");

            _recurringSnapshotCleanup =
                Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(10), Self, ClearSnapshots.Instance, ActorRefs.NoSender);

            base.PreStart();
        }

        protected override void PostStop()
        {
            _recurringSnapshotCleanup?.Cancel();
            base.PostStop();
        }
    }
}
