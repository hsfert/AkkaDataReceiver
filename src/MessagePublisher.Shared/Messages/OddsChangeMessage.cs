using MessagePublisher.Shared.Models;
using System.Collections.Generic;

namespace MessagePublisher.Shared.Messages
{
    public class OddsChangeMessage : IPublisherMessage
    {
        public long SeqNumber { get; private set; }
        public string Queue { get; private set; }
        public long GameId { get; private set; }
        public IReadOnlyList<PoolOddsChange> OddsChange { get; private set; }
        public object ConsistentHashKey { get { return GameId; } }

        public OddsChangeMessage(long seqNumber,
            string queue,
            long gameId,
            IReadOnlyList<PoolOddsChange> oddsChange)
        {
            SeqNumber = seqNumber;
            Queue = queue;
            GameId = gameId;
            OddsChange = oddsChange;
        }

        public IPublisherMessage Clone(long seqNumber, string queue)
        {
            return new OddsChangeMessage(seqNumber, queue, GameId, OddsChange);
        }
    }
}
