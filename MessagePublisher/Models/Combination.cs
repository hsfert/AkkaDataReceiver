using MessagePublisher.Utility;

namespace MessagePublisher.Models
{
    public class Combination
    {
        public short CombinationId { get; set; }
        public decimal Odds { get; set; }
        public decimal Investment { get; set; }
        public decimal Liabilities { get; set; }

        public Combination(short combinationId)
        {
            CombinationId = combinationId;
        }

        public void AddInvestment()
        {
            int sales = RandomGenerator.Instance.Next(1, 3000) * 10;
            Investment += sales;
            Liabilities += sales * Odds;
        }

        public void ChangeOdds()
        {
            Odds = RandomGenerator.Instance.Next(1000, 1000000) / 1000.0m;
        }
    }
}