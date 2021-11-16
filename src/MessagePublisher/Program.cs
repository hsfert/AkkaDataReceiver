using Akka.Actor;
using Akka.Configuration;
using Akka.Routing;
using MessagePublisher.Actors;
using MessagePublisher.Config;
using MessagePublisher.Shared.Actors;
using MessagePublisher.Shared.Utility;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace MessagePublisher
{
    class Program
    {
        static void Main(string[] args)
        {
            var gameConfig = GetGameActorConfig();
            var akkaConfig = GetAkkaConfig(gameConfig);
            using (var actorSystem = ActorSystem.Create("datareceiver", akkaConfig))
            {
                var investmentRouter = actorSystem.ActorOf(Props.Create<MessageQueue>().WithRouter(FromConfig.Instance), "investment-queue");
                var oddsChangeRouter = actorSystem.ActorOf(Props.Create<MessageQueue>().WithRouter(FromConfig.Instance), "oddschange-queue");
                for (int i = 1; i <= gameConfig.NumberOfGames; i++)
                {
                    actorSystem.ActorOf(GameActor.Props(i, investmentRouter, oddsChangeRouter, gameConfig), "game_" + i);
                }
                actorSystem.WhenTerminated.Wait();
            }
        }

        private static GameActorConfig GetGameActorConfig()
        {
            var gameConfig = GameActorConfig.FromConfig(ConfigurationExtractor.Instance.Config);
            return gameConfig;
        }

        private static Akka.Configuration.Config GetAkkaConfig(GameActorConfig gameConfig)
        {
            var configContent = File.ReadAllText("messagepublisher.hocon");
            configContent = configContent.Replace("@@@NumberOfQueuesPerTopic@@@", gameConfig.NumberOfQueuesPerTopic.ToString());
            var akkaConfig = ConfigurationFactory.ParseString(configContent);
            return akkaConfig;
        }
    }
}
