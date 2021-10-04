using MessagePublisher.Shared.Models;

namespace DataReceiver.Shared.Models
{
    public class Combination
    {
        public int GameId { get; private set; }
        public long PoolId { get; private set; }
        public short CombinationId { get; private set; }
        public decimal Sales { get; private set; }
        public decimal Liability { get; private set; }
        public decimal Odds { get; private set; }
        public long InvestmentSeqNumber { get; private set; }
        public long OddsChangeSeqNumber { get; private set; }


        public Combination(GPCKey key)
        {
            GameId = key.GameId;
            PoolId = key.PoolId;
            CombinationId = key.CombinationId;
        }

        public void UpdateInvestment(long seqNumber, CombinationInvestment investment)
        {
            if (InvestmentSeqNumber <= seqNumber)
            {
                Sales = investment.Sales;
                Liability = investment.Liability;
                InvestmentSeqNumber = seqNumber;
            }
        }

        public void UpdateOdds(long seqNumber, CombinationOddsChange oddsChange)
        {
            if (OddsChangeSeqNumber <= seqNumber)
            {
                Odds = oddsChange.OddsAfter;
                OddsChangeSeqNumber = seqNumber;
            }
        }
    }
}
