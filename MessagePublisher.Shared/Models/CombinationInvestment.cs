namespace MessagePublisher.Shared.Models
{
    public class CombinationInvestment
    {
        public GPCKey Key { get; private set; }
        public decimal Sales { get; private set; }
        public decimal Liability { get; private set; }

        public CombinationInvestment(GPCKey key, decimal sales, decimal liability)
        {
            Key = key;
            Sales = sales;
            Liability = liability;
        }
    }
}
