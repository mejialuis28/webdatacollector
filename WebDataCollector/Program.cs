using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using WebDataCollector.Configuration;

namespace WebDataCollector
{
    class Program
    {
        static void Main(string[] args)
        {

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            // gets sites from configuration
            var chartSites = new ChartSites();
            configuration.GetSection("ChartSites").Bind(chartSites);

            // gets cosmos db  configuration
            var cosmosDb = new CosmosDB();
            configuration.GetSection("CosmosDB").Bind(cosmosDb);

            // collects data from shazam
            var shazamCollector = new ShazamDataCollector(chartSites.Shazam, cosmosDb);
            Console.WriteLine("Collecting Shazam data");
            shazamCollector.CollectAndPublish().Wait();
            Console.WriteLine("Collecting Shazam data completed");

            //collects data from youtube
            //Not really, yet to be implemneted
            var youtubeCollector = new YoutubeDataCollector(chartSites.Youtube);
            Console.WriteLine("Collecting YouTube data");
            youtubeCollector.CollectAndPublish().Wait();
            Console.WriteLine("Collecting YouTube data completed");

            Console.ReadLine();
        }
    }
}
