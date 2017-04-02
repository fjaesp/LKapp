using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LK.Helpers
{
    public class FileHandler
    {
        HttpClient webClient;

        public FileHandler()
        {
            var webClient = new HttpClient();
        }

        public async Task<string> GetFileAsync(string url)
        {
            try
            {
                string content = await webClient.GetStringAsync(url);

                return content;
            }
            catch (ArgumentNullException e)
            {
                string aex = e.Message;
            }
            return null;            
        }

    }
}
