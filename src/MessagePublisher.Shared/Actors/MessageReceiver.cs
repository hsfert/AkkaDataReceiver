using Akka;
using Akka.Actor;
using Akka.Routing;
using Akka.Streams;
using Akka.Streams.Dsl;
using MessagePublisher.Shared.Config;
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
        private IActorRef _parent;
        private string _publisherUrl;
        private string[] _routerNames;
        private Dictionary<string, ISourceRef<IPublisherMessage>> _messageSources;
        private Dictionary<string, IActorRef> _watched;
        private int _counter;
        private IRunnableGraph<NotUsed> _graph;
        private ICancelable _recurringQueueFinding;

        public MessageReceiver(IActorRef parent,
            MessageReceiverConfig config)
        {
            _parent = parent;
            _publisherUrl = config.PublisherUrl;
            _routerNames = config.RouterNames;
            _messageSources = new Dictionary<string, ISourceRef<IPublisherMessage>>();
            _watched = new Dictionary<string, IActorRef>();
            _counter = config.NumberOfQueuesPerTopic * _routerNames.Length;
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
                if (_messageSources.ContainsKey(message.QueueName))
                {
                    return;
                }
                _watched[message.QueueName] = message.ActorRef;
                Context.Watch(message.ActorRef);
                _messageSources[message.QueueName] = message.SourceRef;
                if (_counter == _messageSources.Count)
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
            foreach (var actor in _watched.Values)
            {
                Context.Unwatch(actor);
            }
            _watched.Clear();
            _messageSources.Clear();
            _recurringQueueFinding?.Cancel();
            _recurringQueueFinding = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(2), Self, GetSource.Instance, Self);
        }

        private void ReceiveData()
        {
            PrepareForReceivingData();
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
            var sources = _messageSources.Values.Select(src => src.Source).ToArray();
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
                .To(Sink.ActorRefWithAck<IEnumerable<IPublisherMessage>>(_parent, Init.Instance, Ack.Instance, Complete.Instance));
            _graph.Run(Context.System.Materializer());
        }

        protected override void PostStop()
        {
            _recurringQueueFinding?.Cancel();
            base.PostStop();
        }
    }
}
