
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WebDataCollector.Interfaces;
using WebDataCollector.Models;

namespace WebDataCollector
{
    public class ShazamDataCollector : IDataCollector
    {

        private string _chartUrl;

        private readonly static Dictionary<string, string> countries = new Dictionary<string, string>
        {
            { "Global", "world-chart-world"},
            { "US", "country-chart-US"},
            { "IE", "country-chart-IE"},
            { "CO", "country-chart-CO"}
        };

        //constructor
        public ShazamDataCollector(string chartUrl) => _chartUrl = chartUrl;

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
                var title = item["heading"]["title"].ToString();
                var artist = item["heading"]["subtitle"].ToString();
                songs.Add(
                    new Dictionary<string, string>
                    {
                        { "rank", counter.ToString() },
                        { "title", title },
                        { "artist", artist }
                    }
                );

                counter++;
            }

            return songs;
        }


        private readonly string Endpoint = "https://songdatatest.documents.azure.com:443/";
        private readonly string Key = "atFPv1o5m7duF6RSnt2QyEzXOR2PGPBBhuYuF3owlJSuedWSLXUq96Kh3jsTfRvJXcjOrhrSE1Axe0XlMLHjTg==";
        private readonly string DatabaseId = "SongsDataCollector";
        private readonly string CollectionId = "Charts";
        private DocumentClient client;

        public async Task CollectAndPublish() {
            var songs = await Collect();
            var chartHarvest = new ChartHarvest()
            {
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Source = "Shazam",
                Chart = songs
            };
            client = new DocumentClient(new Uri(Endpoint), Key);
            await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), chartHarvest);
        }


    }
    
}
