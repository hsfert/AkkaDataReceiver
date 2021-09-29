using Microsoft.Extensions.Configuration;

namespace DataReceiver.Shared.Config
{
    public class MessageMasterConfig
    {
        public int NumberOfQueuesPerTopic { get; set; }
        public string PublisherUrl { get; set; }
        public string[] RouterNames { get; set; }

        public static MessageMasterConfig FromConfig(IConfiguration config)
        {
            return new MessageMasterConfig
            {
                NumberOfQueuesPerTopic = int.Parse(config["NumberOfQueuesPerTopic"]),
                PublisherUrl = config["PublisherUrl"],
                RouterNames = config["RouterNames"].Split(',')
            };
        }
    }
}
