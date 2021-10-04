using Akka;
using Akka.Actor;
using Akka.Streams;
using Akka.Streams.Dsl;
using MessagePublisher.Shared.Messages;
using MessagePublisher.Shared.Utility;
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
        private RingBuffer<IPublisherMessage> _buffer;

        public MessageQueue()
        {
            _buffer = new RingBuffer<IPublisherMessage>(1000);
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
                _buffer.Add(outputMessage);
                Console.WriteLine(DateTime.Now + " Write a message to queue " + this._queueName);
                await this._queue.OfferAsync(outputMessage);
            });

            Receive<SourceRequest>(request =>
            {
                _source
                .RunWith(StreamRefs.SourceRef<IPublisherMessage>(), Context.System.Materializer())
                .PipeTo(Sender, success: sourceRef => new SourceResponse<IPublisherMessage>(_queueName, Self, sourceRef));
            });

            Receive<RecoveryRequest>(async request =>
            {
                foreach(var info in request.Informations)
                {
                    if(info.Queue == _queueName)
                    {
                        Console.WriteLine(DateTime.Now + " Received recovery request from seq number " + info.StartSeqNumber);
                        var messages = _buffer.GetBufferStartWith(info.StartSeqNumber);
                        Console.WriteLine(DateTime.Now + " Found " + messages.Length + " messages");
                        foreach (var message in messages)
                        {
                            await this._queue.OfferAsync(message);
                        }
                    }
                }
            });

            Receive<StreamInit>(_ =>
            {
                Sender.Tell(StreamAck.Instance);
            });

            Receive<StreamCompleted>(_ =>
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