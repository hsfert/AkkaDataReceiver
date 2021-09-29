using MessagePublisher.Utility;

namespace MessagePublisher.Models
{
    public class Pool
    {
        public int PoolId { get; set; }
        public Combination[] Combinations { get; set; }

        public Pool(int poolId)
        {
            PoolId = poolId;
        }

        public void InitializePool()
        {
            int numberOfSelections = RandomGenerator.Instance.Next(2, 31);
            Combinations = new Combination[numberOfSelections];
            for (int i = 0; i < numberOfSelections; i++)
            {
                Combinations[i] = new Combination((short)(i + 1));
                Combinations[i].ChangeOdds();
            }
        }

        public void AddInvestment()
        {
            int selectionId = RandomGenerator.Instance.Next(0, Combinations.Length);
            Combinations[selectionId].AddInvestment();
        }

        public void ChangeOdds()
        {
            for (int i = 0; i < Combinations.Length; i++)
            {
                Combinations[i].ChangeOdds();
            }
        }
    }
}
