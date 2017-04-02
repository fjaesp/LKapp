using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LK.Models
{
    public class AttachmentEntity
    {
        [JsonProperty("url")]
        public string url { get; set; }

        [JsonProperty("filename")]
        public string filename
        {
            get
            {
                return url.Substring(url.LastIndexOf("|")+1, url.Length - url.LastIndexOf("|")-1);
            }
        }

        [JsonProperty("fileicon")]
        public string fileicon
        {
            get
            {
                string extension = filename.Substring(filename.LastIndexOf(".")+1, filename.Length - filename.LastIndexOf(".")-1);
                
                if (extension == "pdf")
                    return "iconpdf.png";
                else if(extension == "doc" || extension == "docx" || extension == "dot" || extension == "dotm" || extension == "docm")
                    return "iconword.png";
                else if (extension == "xls" || extension == "xlsx" || extension == "xlt" || extension == "xltx" || extension == "xlsm")
                    return "iconexcel.png";

                return string.Empty;
            }
        }
    }
}
