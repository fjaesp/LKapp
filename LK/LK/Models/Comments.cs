using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LK.Models
{
    public class Comments
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("createdAt")]
        public DateTime createdAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime updatedAt { get; set; }

        [JsonProperty("deleted")]
        public bool deleted { get; set; }

        [JsonProperty("userid")]
        public string userid { get; set; }

        [JsonProperty("eventid")]
        public string eventid { get; set; }

        [JsonProperty("comment")]
        public string comment { get; set; }

        [Microsoft.WindowsAzure.MobileServices.Version]
        public string AzureVersion { get; set; }

        public string userName { get; set; }
    }
}
