using Microsoft.Extensions.Configuration;
using System.IO;

namespace AppReadQueueSQS
{
    public static class AppSettings
    {
        private static readonly IConfigurationRoot configuration;

        static AppSettings()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true)
                .Build();
        }

        public static string Get(string key)
        {
            return configuration[key];
        }
    }

}
