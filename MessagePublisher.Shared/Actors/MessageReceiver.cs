using Akka;
using Akka.Actor;
using Akka.Routing;
using Akka.Streams;
using Akka.Streams.Dsl;
using MessagePublisher.Shared.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MessagePublisher.Shared.Actors
{
    /// <summary>
    /// Library provided external component to collect messages from topic queues,
    /// the url, queues and topics are supposed to be known.
    /// </summary>
    public class MessageReceiver : ReceiveActor
    {
        private string _publisherUrl;
        private string[] _routerNames;
        private Dictionary<string, ISourceRef<IPublisherMessage>> messageSources;
        private Dictionary<string, IActorRef> watched;
        private int _counter;
        private IRunnableGraph<NotUsed> _graph;
        private ICancelable _recurringQueueFinding;

        public MessageReceiver(int numberOfQueuesPerTopic, 
            string publisherUrl, 
            string[] routerNames)
        {
            _publisherUrl = publisherUrl;
            _routerNames = routerNames;
            messageSources = new Dictionary<string, ISourceRef<IPublisherMessage>>();
            watched = new Dictionary<string, IActorRef>();
            _counter = numberOfQueuesPerTopic * routerNames.Length;
            Become(WaitingForStreams);
        }

        private void WaitingForStreams()
        {
            PrepareForStream();
            Receive<GetSource>(_ =>
            {
                for (int i = 0; i < _routerNames.Length; i++)
                {
                    var router = Context.ActorSelection(_publisherUrl + "user/" + _routerNames[i]);
                    router.Tell(new Broadcast(SourceRequest.Instance));
                }
            });
            Receive<SourceResponse<IPublisherMessage>>(message =>
            {
                if (messageSources.ContainsKey(message.QueueName))
                {
                    return;
                }
                watched[message.QueueName] = message.ActorRef;
                Context.Watch(message.ActorRef);
                messageSources[message.QueueName] = message.SourceRef;
                if (_counter == messageSources.Count)
                {
                    Become(ReceiveData);
                }
            });
            Receive<Terminated>(message =>
            {
                Become(WaitingForStreams);
            });
        }

        private void PrepareForStream()
        {
            foreach (var actor in watched.Values)
            {
                Context.Unwatch(actor);
            }
            watched.Clear();
            messageSources.Clear();
            _recurringQueueFinding?.Cancel();
            _recurringQueueFinding = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(2), Self, GetSource.Instance, Self);
        }

        private void ReceiveData()
        {
            PrepareForReceivingData();
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
            Receive<Complete>(_ =>
            {
                Become(WaitingForStreams);
            });
            Receive<Terminated>(message =>
            {
                Become(WaitingForStreams);
            });
        }

        private void PrepareForReceivingData()
        {
            _recurringQueueFinding?.Cancel();
            var sources = messageSources.Values.Select(src => src.Source).ToArray();
            Source<IPublisherMessage, NotUsed> aggregateSource;
            if (sources.Length == 1)
            {
                aggregateSource = sources[0];
            }
            else if (sources.Length == 2)
            {
                aggregateSource = Source.Combine(sources[0], sources[1], (i) => new Merge<IPublisherMessage>(i));
            }
            else
            {
                var rest = sources.Skip(2).ToArray();
                aggregateSource = Source.Combine(sources[0], sources[1], (i) => new Merge<IPublisherMessage>(i), rest);
            }
            _graph = aggregateSource
                .GroupedWithin(100, TimeSpan.FromMilliseconds(1000))
                .To(Sink.ActorRefWithAck<IEnumerable<IPublisherMessage>>(Self, Init.Instance, Ack.Instance, Complete.Instance));
            _graph.Run(Context.System.Materializer());
        }

        protected override void PostStop()
        {
            _recurringQueueFinding?.Cancel();
            base.PostStop();
        }
    }
}
