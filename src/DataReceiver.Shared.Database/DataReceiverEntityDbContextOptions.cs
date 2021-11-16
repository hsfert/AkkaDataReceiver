using MessagePublisher.Shared.Utility;
using Microsoft.EntityFrameworkCore;

namespace DataReceiver.Shared.Database
{
    internal class DataReceiverEntityDbContextOptions
    {
        internal DbContextOptionsBuilder<DataReceiverContext> optionBuilder;
        internal static DataReceiverEntityDbContextOptions Instance = new DataReceiverEntityDbContextOptions();

        private DataReceiverEntityDbContextOptions()
        {
            var config = ConfigurationExtractor.Instance.Config;
            string connectionString = config["DataReceiverContext"];
            DbContextOptions<DataReceiverContext> options = new DbContextOptions<DataReceiverContext>();
            optionBuilder = new DbContextOptionsBuilder<DataReceiverContext>();
            optionBuilder.UseSqlServer(connectionString, opt => opt.CommandTimeout(3600));
        }
    }

}
