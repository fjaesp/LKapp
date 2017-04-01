using System;
using Newtonsoft.Json;
using System.Globalization;
using Newtonsoft.Json.Serialization;

namespace LK.Models
{
    public class EventEntities
    {
        [JsonProperty("Id")]
        public string Id { get; set; }
      
        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("PictureUrl")]
        public string PictureUrl { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Date")]
        public DateTime Date { get; set; }

        [JsonProperty("AttachmentUrl")]
        public string AttachmentUrl { get; set; }

        [JsonProperty("Address")]
        public string Address { get; set; }

        [JsonProperty("Topic")]
        public string Topic { get; set; }

        [Microsoft.WindowsAzure.MobileServices.Version]
        public string AzureVersion { get; set; }

        [JsonProperty("GroupName")]
        public string MonthGroupName
        {
            get
            {
                CultureInfo ci = new CultureInfo("nb-NO");
                return string.Format("{0} {1}", ci.DateTimeFormat.GetMonthName(Date.Month).ToUpper(), Date.Year.ToString());
            }
        }

        [JsonProperty("CurrentUserAttend")]
        public bool CurrentUserAttend { get; set; }
    }
}
