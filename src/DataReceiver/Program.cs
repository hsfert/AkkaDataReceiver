using Akka.Actor;
using Akka.Cluster.Tools.Singleton;
using Akka.Configuration;
using Akka.Routing;
using DataReceiver.Shared.Actors;
using MessagePublisher.Shared.Config;
using MessagePublisher.Shared.Utility;
using System.IO;

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
                var poolRouter = actorSystem.ActorOf(
                    Props.Create(() => new PoolSnapshotActor())
                                .WithRouter(FromConfig.Instance), "pool-snapshot");

                var poolMessageRouter =
                        actorSystem.ActorOf(
                            Props.Create(() => new PoolMessageDispatcherActor(poolRouter))
                                .WithRouter(FromConfig.Instance), "pool-message-dispatcher");
               
                var messageMasterProps = Props.Create(() => new MessageMaster(masterConfig, poolMessageRouter));

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
            var masterConfig = MessageReceiverConfig.FromConfig(ConfigurationExtractor.Instance.Config);
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
