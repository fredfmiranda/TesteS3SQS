using Amazon.SQS;
using Amazon.SQS.Model;
using AppReadQueueSQS;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.IO;

namespace SQSConsumer
{
    class Program
    {
        public static string connectionString = string.Empty;
        static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<ISQSManager, AmazonSQSManager>()
                .AddSingleton<IDatabaseManager, MySqlDatabaseManager>()
                .AddSingleton<App>()
                .BuildServiceProvider();

            var app = serviceProvider.GetService<App>();
            await app.Run();
        }
    }

}
