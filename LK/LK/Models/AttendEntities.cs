using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LK.Models
{
    public class AttendEntities
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("createdAt")]
        public string createdAt { get; set; }

        [JsonProperty("updatedAt")]
        public string updatedAt { get; set; }

        [Microsoft.WindowsAzure.MobileServices.Version]
        public string AzureVersion { get; set; }

        [JsonProperty("deleted")]
        public string deleted { get; set; }

        [JsonProperty("eventid")]
        public string eventid { get; set; }

        [JsonProperty("userid")]
        public string userid { get; set; }
    }
}
