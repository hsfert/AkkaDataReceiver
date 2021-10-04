using Akka.Routing;

namespace DataReceiver.Shared.Messages
{
    public interface IPoolMessage : IConsistentHashable
    {
        long SeqNumber { get; }
        int GameId { get; }
        long PoolId { get; }
        PoolMessageType MessageType { get; }
    }
}
