using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessagePublisher.Shared.Utility
{
    public class ConfigurationExtractor
    {
        public static ConfigurationExtractor Instance = new ConfigurationExtractor();

        public IConfiguration Config { get; private set; }
        private ConfigurationExtractor()
        {
            var builder = Host.CreateDefaultBuilder(null)
                .ConfigureAppConfiguration((host, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{host.HostingEnvironment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
                });

             Config = builder.Build()
                .Services.GetRequiredService<IConfiguration>();
        }
    }
}
