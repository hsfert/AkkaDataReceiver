using Akka.Actor;
using Akka.Configuration;
using System.IO;

namespace LightHouse
{
    class Program
    {
        static void Main(string[] args)
        {
            var configContent = File.ReadAllText("lighthouse.hocon");
            var config = ConfigurationFactory.ParseString(configContent);
            using (var actorSystem = ActorSystem.Create("datareceiver", config))
            {
                actorSystem.WhenTerminated.Wait();
            }
        }
    }
}
