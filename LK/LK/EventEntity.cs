using System;


namespace LK
{
    public class EventEntities
    {
        //string id;
        //string title;
        //string pictureUrl;
        //string description;
        //DateTime date;
        //string attachementUrl;
        //string address;

        [Newtonsoft.Json.JsonProperty("Id")]
        public string Id { get; set; }
      
        [Newtonsoft.Json.JsonProperty("Title")]
        public string Title { get; set; }

        [Newtonsoft.Json.JsonProperty("PictureUrl")]
        public string PictureUrl { get; set; }

        [Newtonsoft.Json.JsonProperty("Description")]
        public string Description { get; set; }

        [Newtonsoft.Json.JsonProperty("Date")]
        public DateTime Date { get; set; }

        //[Newtonsoft.Json.JsonProperty("Date2String")]
        //public string Date2String = Date.ToString() { get; set; }


        [Newtonsoft.Json.JsonProperty("AttachmentUrl")]
        public string AttachmentUrl { get; set; }

        [Newtonsoft.Json.JsonProperty("Address")]
        public string Address { get; set; }

        [Microsoft.WindowsAzure.MobileServices.Version]
        public string AzureVersion { get; set; }
    }
}
