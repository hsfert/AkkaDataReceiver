using MessagePublisher.Shared.Models;
using System.Collections.Generic;

namespace DataReceiver.Shared.Messages
{
    public class PoolInvestmentUpdateMessage : IPoolMessage
    {
        public long SeqNumber { get; private set; }
        public int GameId { get; private set; }
        public long PoolId { get; private set; }
        public IReadOnlyList<CombinationInvestment> Combinations {get; private set;}
        public PoolMessageType MessageType { get; private set; }
        public object ConsistentHashKey => PoolId;

        public PoolInvestmentUpdateMessage(long seqNumber, int gameId, long poolId, IReadOnlyList<CombinationInvestment> combinations)
        {
            SeqNumber = seqNumber;
            GameId = gameId;
            PoolId = poolId;
            Combinations = combinations;
            MessageType = PoolMessageType.Investment;
        }
    }
}
