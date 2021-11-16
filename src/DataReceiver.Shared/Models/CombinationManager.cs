using DbCombination = DataReceiver.Shared.Database.Combination;
using MessagePublisher.Shared.Models;

namespace DataReceiver.Shared.Models
{
    public class CombinationManager
    {
        public long Id { get; private set; }
        public int GameId { get; private set; }
        public long PoolId { get; private set; }
        public short CombinationId { get; private set; }
        public decimal Sales { get; private set; }
        public decimal Liability { get; private set; }
        public decimal? Odds { get; private set; }
        public long InvestmentSeqNumber { get; private set; }
        public long OddsChangeSeqNumber { get; private set; }
        private bool _updated;

        public CombinationManager(GPCKey key, DbCombination combination)
        {
            Id = combination.id;
            GameId = key.GameId;
            PoolId = key.PoolId;
            CombinationId = key.CombinationId;
            Sales = combination.sales;
            Liability = combination.liability;
            Odds = combination.odds;
            InvestmentSeqNumber = combination.investment_number;
            OddsChangeSeqNumber = combination.odds_number;
            _updated = false;
        }

        public CombinationManager(long id, GPCKey key)
        {
            Id = id;
            GameId = key.GameId;
            PoolId = key.PoolId;
            CombinationId = key.CombinationId;
        }

        public bool UpdateInvestment(long seqNumber, CombinationInvestment investment)
        {
            if (InvestmentSeqNumber > seqNumber)
            {
                return false;
            }
            Sales = investment.Sales;
            Liability = investment.Liability;
            InvestmentSeqNumber = seqNumber;
            _updated = true;
            return true;
        }

        public bool UpdateOdds(long seqNumber, CombinationOddsChange oddsChange)
        {
            if (OddsChangeSeqNumber > seqNumber)
            {
                return false;
            }
            Odds = oddsChange.OddsAfter;
            OddsChangeSeqNumber = seqNumber;
            _updated = true;
            return true;
        }

        public bool GetUpdatedEntry(out DbCombination output)
        {
            output = null;
            if (!_updated)
            {
                return false;
            }
            output = new DbCombination
            {
                id = Id,
                sales = Sales,
                liability = Liability,
                odds = Odds,
                investment_number = InvestmentSeqNumber,
                odds_number = OddsChangeSeqNumber
            };
            return true;
        }
    }
}
