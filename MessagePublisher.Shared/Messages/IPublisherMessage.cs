using Akka.Routing;
namespace MessagePublisher.Shared.Messages
{
    public interface IPublisherMessage  : IConsistentHashable
    {
        long SeqNumber { get; }
        string Queue { get; }
        IPublisherMessage Clone(long seqNumber, string queue);
    }
}
