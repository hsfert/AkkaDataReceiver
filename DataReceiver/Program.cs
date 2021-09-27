using Akka.Actor;
using Akka.Configuration;
using MessagePublisher.Shared.Actors;
using System.Configuration;
using System.IO;

namespace DataReceiver
{
    class Program
    {
        static void Main(string[] args)
        {
            int numberOfQueuesPerTopic = int.Parse(ConfigurationManager.ConnectionStrings["NumberOfQueuesPerTopic"].ConnectionString);
            var configContent = File.ReadAllText("messagereceiver.hocon");
            var config = ConfigurationFactory.ParseString(configContent);
            using (var actorSystem = ActorSystem.Create("datareceiver", config))
            {
                actorSystem.ActorOf(Props.Create(() => new MessageReceiver(numberOfQueuesPerTopic, 
                    "akka.tcp://datareceiver@localhost:4053/", 
                    new string[] {
                    "investment-queue",
                    "oddschange-queue"
                })));
                actorSystem.WhenTerminated.Wait();
            }
        }
    }
}
