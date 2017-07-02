using Newtonsoft.Json;
using System;

namespace LK.Models
{
    public class NotificationEntity
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

        [JsonProperty("Message")]
        public string Message { get; set; }

        [JsonProperty("NotificationType")]
        public string NotificationType { get; set; }

        [JsonProperty("EventID")]
        public string EventID { get; set; }

        [JsonProperty("UserID")]
        public string UserID { get; set; }
    }
}
