namespace MessagePublisher.Shared.Models
{
    public class CombinationOddsChange
    {
        public GPCKey Key { get; private set; }
        public decimal OddsBefore { get; private set; }
        public decimal OddsAfter { get; private set; }

        public CombinationOddsChange(GPCKey key, decimal oddsBefore, decimal oddsAfter)
        {
            Key = key;
            OddsBefore = oddsBefore;
            OddsAfter = oddsAfter;
        }
    }
}