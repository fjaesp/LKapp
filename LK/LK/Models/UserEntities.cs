using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LK.Models
{
    public class UserEntities
    {
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("UpdatedAt")]
        public DateTime UpdatedAt { get; set; }

        [Microsoft.WindowsAzure.MobileServices.Version]
        public string AzureVersion { get; set; }

        [JsonProperty("deleted")]
        public bool deleted { get; set; }

        [JsonProperty("mail")]
        public string mail { get; set; }

        [JsonProperty("displayName")]
        public string displayName { get; set; }

        [JsonProperty("topic")]
        public string topic { get; set; }

        [JsonProperty("basis")]
        public string basis { get; set; }

        [JsonProperty("active")]
        public string active { get; set; }

        [JsonProperty("userPrincipalName")]
        public string userPrincipalName { get; set; }

        [JsonProperty("telephone")]
        public string telephone { get; set; }

        [JsonProperty("SPGUID")]
        public string SPGUID { get; set; }

        [JsonProperty("AttachmentUrl")]
        public string AttachmentUrl { get; set; }

		[JsonProperty("installationid")]
		public string installationid { get; set; }

		[JsonProperty("profilepictureurl")]
		public string profilepictureurl { get; set; }
    }
}
