using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LK.Models
{
    public class NotificationEntities
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

        [JsonProperty("userid")]
        public string userid { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("read")]
        public bool read { get; set; }

        [JsonProperty("sent")]
        public bool sent { get; set; }

        [JsonProperty("notificationtype")]
        public string notificationtype { get; set; }

        [JsonProperty("eventid")]
        public string eventid { get; set; }


    }
}
