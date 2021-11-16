using DbCombination = DataReceiver.Shared.Database.Combination;
using MessagePublisher.Shared.Models;
using System.Collections.Generic;
using System.Linq;

namespace DataReceiver.Shared.Models
{
    public class PoolManager
    {
        public int GameId { get; private set; }
        public long PoolId { get; private set; }
        private CombinationDatabaseDictionary _combinations;
        private bool _updated;

        public PoolManager(long poolId, int gameId)
        {
            _updated = false;
            PoolId = poolId;
            GameId = gameId;
            _combinations = new CombinationDatabaseDictionary(gameId);
        }

        public void UpdateInvestment(long seqNumber, 
            List<CombinationInvestment> investments)
        {
            var combinations = _combinations.GetOrAddCombinations(investments.Select(investment => investment.Key).ToList());
            CombinationManager combination; 
            foreach (var investment in investments)
            {
                GPCKey key = investment.Key;
                combination = combinations[key];
                _updated = combination.UpdateInvestment(seqNumber, investment) || _updated;
            }
        }

        public void UpdateOdds(long seqNumber, List<CombinationOddsChange> oddsChanges)
        {
            var combinations = _combinations.GetOrAddCombinations(oddsChanges.Select(oddsChange => oddsChange.Key).ToList());
            CombinationManager combination;
            foreach (var oddsChange in oddsChanges)
            {
                GPCKey key = oddsChange.Key;
                combination = combinations[key];
                _updated = combination.UpdateOdds(seqNumber, oddsChange) || _updated;
            }
        }

        public bool GetUpdatedEntries(out List<DbCombination> outputs)
        {
            outputs = new List<DbCombination>();
            if (!_updated)
            {
                return false;
            }

            foreach (CombinationManager combination in _combinations.GetAllCombinations())
            {
                DbCombination output;
                if(combination.GetUpdatedEntry(out output))
                {
                    outputs.Add(output);
                }
            }
            _updated = false;
            return true;
        }
    }
}
