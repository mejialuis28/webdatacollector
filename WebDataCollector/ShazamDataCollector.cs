
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WebDataCollector.Configuration;
using WebDataCollector.Interfaces;
using WebDataCollector.Models;

namespace WebDataCollector
{
    public class ShazamDataCollector : IDataCollector
    {

        private string _chartUrl;
        private CosmosDB _cosmosDB;

        private readonly static Dictionary<string, string> countries = new Dictionary<string, string>
        {
            { "Global", "world-chart-world"},
            { "US", "country-chart-US"},
            { "IE", "country-chart-IE"},
            { "CO", "country-chart-CO"}
        };

        //constructor
        public ShazamDataCollector(string chartUrl, CosmosDB cosmos)
        {
            _chartUrl = chartUrl;
            _cosmosDB = cosmos;
        }

        public async Task<List<Dictionary<string, string>>> Collect()
        {
            var client = new HttpClient();
            var url = _chartUrl.Replace("{country}", countries.GetValueOrDefault("Global"));
            var jsonResponse = await client.GetStringAsync(url);

            var jsonTokens = JObject.Parse(jsonResponse);
            var tracks = (JArray)jsonTokens["chart"];

            var songs = new List<Dictionary<string, string>>();
            int counter = 1;
            foreach (var item in tracks)
            {
                songs.Add(
                    new Dictionary<string, string>
                    {
                        { "rank", counter.ToString() },
                        { "title", item["heading"]["title"].ToString() },
                        { "artist", item["heading"]["subtitle"].ToString() }
                    }
                );
                counter++;
            }

            return songs;
        }

        public async Task CollectAndPublish() {
            var songs = await Collect();
            var chartHarvest = new ChartHarvest()
            {
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Source = "Shazam",
                Chart = songs
            };
            var client = new DocumentClient(new Uri(_cosmosDB.Endpoint), _cosmosDB.Key);
            await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_cosmosDB.Database, "Charts"), chartHarvest);
        }


    }
    
}
