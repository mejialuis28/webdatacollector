using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebDataCollector.Interfaces;

namespace WebDataCollector
{
    public class YoutubeDataCollector : IDataCollector
    {

        private string _chartUrl;

        private readonly static Dictionary<string, string> countries = new Dictionary<string, string>
        {
            { "Global", "global" },
            { "US", "us"},
            { "IE", "ie"},
            { "CO", "co"}
        };

        //constructor
        public YoutubeDataCollector(string chartUrl) => _chartUrl = chartUrl;

        public async Task<List<Dictionary<string, string>>> Collect()
        {
            var client = new HttpClient();
            var url = _chartUrl.Replace("{country}", countries.GetValueOrDefault("Global"));
            var siteHtml = await client.GetStringAsync(url);

            // pending check html to get the charts
            var parser = new HtmlParser();
            var document = parser.ParseDocument(siteHtml);

            return null;
        }

        public async Task CollectAndPublish()
        {
            // implementation pending
            return;
        }
    }
}
