using Microsoft.Extensions.Configuration;

namespace MessagePublisher.Config
{
    public class GameActorConfig
    {
        public int NumberOfGames { get; set; }
        public int NumberOfQueuesPerTopic { get; set; }
        public double FrequencyOfInvestmentPerSecond { get; set; }
        public double FrequencyOfOddsChangePerMinute { get; set; }
        public double FrequencyOfPublishingInvestmentSnapshotPerMinute { get; set; }

        public static GameActorConfig FromConfig(IConfiguration config)
        {
            return new GameActorConfig
            {
                NumberOfGames = int.Parse(config["NumberOfGames"]),
                NumberOfQueuesPerTopic = int.Parse(config["NumberOfQueuesPerTopic"]),
                FrequencyOfInvestmentPerSecond = double.Parse(config["FrequencyOfInvestmentPerSecond"]),
                FrequencyOfOddsChangePerMinute = double.Parse(config["FrequencyOfOddsChangePerMinute"]),
                FrequencyOfPublishingInvestmentSnapshotPerMinute = double.Parse(config["FrequencyOfPublishingInvestmentSnapshotPerMinute"])
            };
        }
    }
}
