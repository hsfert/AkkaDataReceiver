using Akka.Actor;
using Akka.Persistence;
using DataReceiver.Shared.Messages;
using DataReceiver.Shared.Models;
using MessagePublisher.Shared.Messages;
using System;
using System.Collections.Generic;

namespace DataReceiver.Shared.Actors
{
    public class PoolMessageDispatcherActor : AtLeastOnceDeliveryReceiveActor
    {
        public override string PersistenceId => Context.Self.Path.Name;
        private Dictionary<PoolMessageMeta, long> _externalIDLookup;
        private Dictionary<PoolMessageMeta, PublisherDoneTask> _doneTasks;
        private PoolMessageDispatcher _dispatcher;
        private ICancelable _recurringSnapshotCleanup;
        private IActorRef _poolActor;

        public PoolMessageDispatcherActor(IActorRef poolActor)
        {
            _dispatcher = new PoolMessageDispatcher();
            _externalIDLookup = new Dictionary<PoolMessageMeta, long>();
            _doneTasks = new Dictionary<PoolMessageMeta, PublisherDoneTask>();
            _poolActor = poolActor;
            Become(ReceiveMessage);
        }

        private void ReceiveMessage()
        {
            Command<IPublisherMessage>(message =>
            {
                var header = new PublisherMessageMeta(message.SeqNumber, message.Queue);

                var poolMessages = _dispatcher.RedistributeMessage(message);

                PublisherDoneTask task = new PublisherDoneTask(Sender, header, poolMessages.Count);

                foreach (var poolMessage in poolMessages)
                {
                    PoolMessageMeta poolHeader = new PoolMessageMeta(poolMessage.SeqNumber, poolMessage.PoolId, poolMessage.MessageType);
                    _doneTasks[poolHeader] = task;

                    Deliver(_poolActor.Path, messageId =>
                    {
                        _externalIDLookup[poolHeader] = messageId;
                        return poolMessage;
                    });
                }
            });

            Command<ReliableDeliveryAck<PoolMessageMeta>>(ack =>
            {
                long internalMessageID;
                if (_externalIDLookup.TryGetValue(ack.MessageMeta, out internalMessageID))
                {
                    ConfirmDelivery(internalMessageID);
                    _externalIDLookup.Remove(ack.MessageMeta);
                }

                PublisherDoneTask task;
                if(_doneTasks.TryGetValue(ack.MessageMeta, out task))
                {
                    if(task.DoTask(ack.MessageMeta))
                    {
                        var pools = task.Pools;
                        foreach(var pool in pools)
                        {
                            _doneTasks.Remove(pool);
                        }
                        var sender = task.Sender;
                        sender.Tell(new ReliableDeliveryAck<PublisherMessageMeta>(task.Meta));
                    }
                }
            });

            Command<ClearSnapshots>(_ =>
            {
                var snapshot = GetDeliverySnapshot().UnconfirmedDeliveries;
                List<long> settledKeys = new List<long>();
                foreach (var delivery in snapshot)
                {
                    var targetPath = delivery.Destination.Name;
                    IPoolMessage externalMessage = (IPoolMessage)delivery.Message;
                    var header = new PoolMessageMeta(externalMessage.SeqNumber, externalMessage.PoolId, externalMessage.MessageType);
                    if (!_externalIDLookup.ContainsKey(header))
                    {
                        ConfirmDelivery(delivery.DeliveryId);
                    }
                }
            });
        }

        protected override void PreStart()
        {
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
