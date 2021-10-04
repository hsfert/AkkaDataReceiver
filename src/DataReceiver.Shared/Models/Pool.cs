using MessagePublisher.Shared.Models;
using System.Collections.Generic;

namespace DataReceiver.Shared.Models
{
    public class Pool
    {
        public int GameId { get; private set; }
        public long PoolId { get; private set; }
        private Dictionary<GPCKey, Combination> Combinations;

        public Pool(long poolId, int gameId)
        {
            PoolId = poolId;
            GameId = gameId;
            Combinations = new Dictionary<GPCKey, Combination>();
        }

        public void UpdateInvestment(long seqNumber, List<CombinationInvestment> investments)
        {
            Combination combination; 
            foreach (var investment in investments)
            {
                GPCKey key = investment.Key;
                if (!Combinations.TryGetValue(key, out combination))
                {
                    combination = new Combination(key);
                    Combinations[key] = combination;
                }
                combination.UpdateInvestment(seqNumber, investment);
            }
        }

        public void UpdateOdds(long seqNumber, List<CombinationOddsChange> oddsChanges)
        {
            Combination combination;
            foreach (var oddsChange in oddsChanges)
            {
                GPCKey key = oddsChange.Key;
                if (!Combinations.TryGetValue(key, out combination))
                {
                    combination = new Combination(key);
                    Combinations[key] = combination;
                }
                combination.UpdateOdds(seqNumber, oddsChange);
            }
        }
    }
}
