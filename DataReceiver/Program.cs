using Akka.Actor;
using Akka.Configuration;
using MessagePublisher.Shared.Actors;
using System.Configuration;
using System.IO;
using DataReceiver.Shared.Actors;
using Akka.Cluster.Tools.Singleton;
using System;

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
                var messageMasterProps = Props.Create(() => new MessageMaster(numberOfQueuesPerTopic,
                    "akka.tcp://datareceiver@localhost:4053/",
                    new string[] {
                    "investment-queue",
                    "oddschange-queue"
                }));

                var master = actorSystem.ActorOf(ClusterSingletonManager.Props(
                    singletonProps: messageMasterProps,
                    terminationMessage: PoisonPill.Instance,
                    settings: ClusterSingletonManagerSettings.Create(actorSystem).WithRole("messagereceiver")),
                    name: "message-master");

                actorSystem.WhenTerminated.Wait();
            }
        }
    }
}
