using Akka.Actor;
using Akka.Configuration;
using Akka.Routing;
using MessagePublisher.Actors;
using MessagePublisher.Shared.Actors;
using System.Configuration;
using System.IO;

namespace MessagePublisher
{
    class Program
    {
        static void Main(string[] args)
        {
            int numberOfGames = int.Parse(ConfigurationManager.ConnectionStrings["NumberOfGames"].ConnectionString);
            int numberOfQueuesPerTopic = int.Parse(ConfigurationManager.ConnectionStrings["NumberOfQueuesPerTopic"].ConnectionString);
            var configContent = File.ReadAllText("messagepublisher.hocon");
            configContent = configContent.Replace("@@@NumberOfQueuesPerTopic@@@", numberOfQueuesPerTopic.ToString());
            var config = ConfigurationFactory.ParseString(configContent);
            using (var actorSystem = ActorSystem.Create("datareceiver", config))
            {
                var investmentRouter = actorSystem.ActorOf(Props.Create<MessageQueue>().WithRouter(FromConfig.Instance), "investment-queue");
                var oddsChangeRouter = actorSystem.ActorOf(Props.Create<MessageQueue>().WithRouter(FromConfig.Instance), "oddschange-queue");
                for (int i = 1; i <= numberOfGames; i++)
                {
                    actorSystem.ActorOf(GameActor.Props(i, investmentRouter, oddsChangeRouter), "game_" + i);
                }
                actorSystem.WhenTerminated.Wait();
            }
        }
    }
}
