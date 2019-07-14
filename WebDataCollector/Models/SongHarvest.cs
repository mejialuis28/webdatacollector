using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebDataCollector.Models
{
    public class ChartHarvest
    {

        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "date")]
        public DateTime Date { get; set; }

        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }

        [JsonProperty(PropertyName = "chart")]
        public List<Dictionary<string, string>> Chart { get; set; }
    }
}
