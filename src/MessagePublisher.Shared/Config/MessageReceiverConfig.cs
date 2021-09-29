using Microsoft.Extensions.Configuration;

namespace MessagePublisher.Shared.Config
{
    public class MessageReceiverConfig
    {
        public int NumberOfQueuesPerTopic { get; set; }
        public string PublisherUrl { get; set; }
        public string[] RouterNames { get; set; }

        public static MessageReceiverConfig FromConfig(IConfiguration config)
        {
            return new MessageReceiverConfig
            {
                NumberOfQueuesPerTopic = int.Parse(config["NumberOfQueuesPerTopic"]),
                PublisherUrl = config["PublisherUrl"],
                RouterNames = config["RouterNames"].Split(',')
            };
        }
    }
}
