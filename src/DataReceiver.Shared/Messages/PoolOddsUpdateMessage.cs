using MessagePublisher.Shared.Models;
using System.Collections.Generic;

namespace DataReceiver.Shared.Messages
{
    public class PoolOddsUpdateMessage : IPoolMessage
    {
        public long SeqNumber { get; private set; }
        public int GameId { get; private set; }
        public long PoolId { get; private set; }
        public PoolMessageType MessageType { get; private set; }
        public IReadOnlyList<CombinationOddsChange> Combinations { get; private set; }
        public object ConsistentHashKey => PoolId;

        public PoolOddsUpdateMessage(long seqNumber, int gameId, long poolId, IReadOnlyList<CombinationOddsChange> combinations)
        {
            SeqNumber = seqNumber;
            GameId = gameId;
            PoolId = poolId;
            Combinations = combinations;
            MessageType = PoolMessageType.OddsChange;
        }
    }
}
