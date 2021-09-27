using Akka;
using Akka.Actor;
using Akka.Streams;
using Akka.Streams.Dsl;
using MessagePublisher.Shared.Messages;
using System;

namespace MessagePublisher.Shared.Actors
{
    /// <summary>
    /// Simulates an external message queue with topic,
    /// if the message is not drained,
    /// it will cause backpressure on the external system.
    /// </summary>
    public class MessageQueue : ReceiveActor
    {
        private string _queueName;
        private long _seqNumber;
        private ISourceQueueWithComplete<IPublisherMessage> _queue;
        private Source<IPublisherMessage, NotUsed> _source;

        public MessageQueue()
        {
            this._queueName = Self.Path.ToString();
            (this._queue, this._source) = Source.Queue<IPublisherMessage>(100, OverflowStrategy.Backpressure)
                .PreMaterialize(Context.System.Materializer());
            Become(PublishMessages);
        }

        private void PublishMessages()
        {
            Receive<IPublisherMessage>(async message =>
            {
                var outputMessage = message.Clone(_seqNumber++, (string)this._queueName);
                Console.WriteLine(DateTime.Now + " Write a message to queue " + this._queueName);
                await this._queue.OfferAsync(outputMessage);
            });

            Receive<SourceRequest>(request =>
            {
                _source
                .RunWith(StreamRefs.SourceRef<IPublisherMessage>(), Context.System.Materializer())
                .PipeTo(Sender, success: sourceRef => new SourceResponse<IPublisherMessage>(_queueName, Self, sourceRef));
            });

            Receive<Init>(_ =>
            {
                Sender.Tell(Ack.Instance);
            });

            Receive<Complete>(_ =>
            {
                this._queue.Complete();
            });
        }

        protected override void PostStop()
        {
            this._queue.Complete();
        }
    }
}