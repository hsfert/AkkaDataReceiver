using MessagePublisher.Utility;

namespace MessagePublisher.Models
{
    public class Game
    {
        public int GameId { get; set; }
        public Pool[] Pools { get; set; }

        public Game(int gameId)
        {
            GameId = gameId;
        }

        public void InitializeGame()
        {
            int numberOfPools = RandomGenerator.Instance.Next(3, 10);
            Pools = new Pool[numberOfPools];
            for (int i = 0; i < numberOfPools; i++)
            {
                int poolId = PoolIdGenerator.Instance.CreatePoolId();
                Pools[i] = new Pool(poolId);
                Pools[i].InitializePool();
            }
        }

        public void AddInvestment()
        {
            int poolId = RandomGenerator.Instance.Next(0, Pools.Length);
            Pools[poolId].AddInvestment();
        }

        public void ChangeOdds()
        {
            for (int i = 0; i < Pools.Length; i++)
            {
                Pools[i].ChangeOdds();
            }
        }
    }
}
