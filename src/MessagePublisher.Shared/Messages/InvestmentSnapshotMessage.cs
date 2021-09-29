using MessagePublisher.Shared.Models;
using System.Collections.Generic;

namespace MessagePublisher.Shared.Messages
{
    public class InvestmentSnapshotMessage : IPublisherMessage
    {
        public long SeqNumber { get; private set; }
        public string Queue { get; private set; }
        public int GameId { get; private set; }
        public List<PoolInvestment> Investments { get; private set; }
        public object ConsistentHashKey { get { return GameId; } }

        public InvestmentSnapshotMessage(long seqNumber,
            string queue,
            int gameId,
            List<PoolInvestment> investments)
        {
            SeqNumber = seqNumber;
            Queue = queue;
            GameId = gameId;
            Investments = investments;
        }

        public IPublisherMessage Clone(long seqNumber, string queue)
        {
            return new InvestmentSnapshotMessage(seqNumber, queue, GameId, Investments);
        }
    }
}
