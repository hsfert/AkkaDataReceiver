using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using MessagePublisher.Shared.Actors;
using MessagePublisher.Shared.Messages;

namespace DataReceiver.Shared.Actors
{
    public class MessageMaster : ReceiveActor
    {
        private IActorRef _messageReceiver;
        private int _numberOfQueuesPerTopic;
        private string _publisherUrl;
        private string[] _routerNames;

        public MessageMaster(int numberOfQueuesPerTopic,
            string publisherUrl,
            string[] routerNames)
        {
            _numberOfQueuesPerTopic = numberOfQueuesPerTopic;
            _publisherUrl = publisherUrl;
            _routerNames = routerNames;
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
                    _numberOfQueuesPerTopic,
                    _publisherUrl,
                    _routerNames)), "message-receiver");
            base.PreStart();
        }
    }
}
