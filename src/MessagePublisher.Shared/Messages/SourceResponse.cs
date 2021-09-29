using Akka.Actor;
using Akka.Streams;

namespace MessagePublisher.Shared.Messages
{
    internal class SourceResponse<T>
    {
        public string QueueName { get; private set; }
        public ISourceRef<T> SourceRef { get; private set; }
        public IActorRef ActorRef { get; private set; }

        public SourceResponse(string queueName, IActorRef actorRef, ISourceRef<T> sourceRef)
        {
            QueueName = queueName;
            ActorRef = actorRef;
            SourceRef = sourceRef;
        }
    }
}
