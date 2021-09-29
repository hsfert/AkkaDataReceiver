using System;
using System.Collections.Generic;
using Akka.Actor;
using MessagePublisher.Shared.Actors;
using MessagePublisher.Shared.Config;
using MessagePublisher.Shared.Messages;

namespace DataReceiver.Shared.Actors
{
    public class MessageMaster : ReceiveActor
    {
        private IActorRef _messageReceiver;
        private MessageReceiverConfig _config;
        private string _publisherUrl;
        private string[] _routerNames;

        public MessageMaster(MessageReceiverConfig config)
        {
            _config = config;
            Become(ReceiveMessage);
        }

        private void ReceiveMessage()
        {
            Receive<IEnumerable<IPublisherMessage>>(messages =>
            {
                foreach (var message in messages)
                {
                    Console.WriteLine("Received Message from " + message.Queue + " with sequence number " + message.SeqNumber);
                }
                Sender.Tell(Ack.Instance);
            });
            Receive<Init>(_ =>
            {
                Sender.Tell(Ack.Instance);
            });
        }

        protected override void PreStart()
        {
            _messageReceiver = Context.ActorOf(Props.Create(() => new MessageReceiver(Self,
                    _config)), "message-receiver");
            base.PreStart();
        }
    }
}
