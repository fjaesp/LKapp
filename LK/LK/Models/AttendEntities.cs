using Newtonsoft.Json;
using System;

namespace LK.Models
{
    public class AttendEntities
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("createdAt")]
        public DateTime createdAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime updatedAt { get; set; }

        [Microsoft.WindowsAzure.MobileServices.Version]
        public string AzureVersion { get; set; }

        [JsonProperty("deleted")]
        public bool deleted { get; set; }

        [JsonProperty("eventid")]
        public string eventid { get; set; }

        [JsonProperty("userid")]
        public string userid { get; set; }
    }
}
