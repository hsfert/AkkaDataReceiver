using System.Collections.Generic;

namespace MessagePublisher.Shared.Models
{
    public class PoolOddsChange
    {
        public long GameId { get; private set; }
        public int PoolId { get; private set; }
        public List<CombinationOddsChange> Combinations { get; private set; }

        public PoolOddsChange(long gameId, int poolId, List<CombinationOddsChange> combinations)
        {
            GameId = gameId;
            PoolId = poolId;
            Combinations = combinations;
        }
    }
}
