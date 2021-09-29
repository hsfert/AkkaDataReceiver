using Akka.Actor;
using Akka.Configuration;
using System.IO;
using Akka.Cluster.Tools.Singleton;
using DataReceiver.Shared.Actors;
using MessagePublisher.Shared.Config;
using Microsoft.Extensions.Configuration;

namespace DataReceiver
{
    class Program
    {
        static void Main(string[] args)
        {
            var masterConfig = GetMessageMasterConfig();
            var akkaConfig = GetAkkaConfig();

            using (var actorSystem = ActorSystem.Create("datareceiver", akkaConfig))
            {
                var messageMasterProps = Props.Create(() => new MessageMaster(masterConfig));

                var master = actorSystem.ActorOf(ClusterSingletonManager.Props(
                    singletonProps: messageMasterProps,
                    terminationMessage: PoisonPill.Instance,
                    settings: ClusterSingletonManagerSettings.Create(actorSystem).WithRole("messagereceiver")),
                    name: "message-master");

                actorSystem.WhenTerminated.Wait();
            }
        }

        private static MessageReceiverConfig GetMessageMasterConfig()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
               .AddJsonFile($"appsettings.json", true, true);
            var config = builder.Build();
            var masterConfig = MessageReceiverConfig.FromConfig(config);
            return masterConfig;
        }

        private static Akka.Configuration.Config GetAkkaConfig()
        {
            var configContent = File.ReadAllText("messagereceiver.hocon");
            var config = ConfigurationFactory.ParseString(configContent);
            return config;
        }
    }
}
