using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;

namespace LKapp.WebJob
{
    public class EventEntity
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string PictureUrl { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string AttachmentUrl { get; set; }
        public string Address { get; set; }
        //public string Attendees { get; set; }

        public override string ToString()
        {
            return "Event [Title=" + this.Title + "]";
        }
    }
}