using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace LKapp.WebJob
{
    public class ReadListItems
    {
        public static string SPAccount = "Fjalestad@Fjalestad2016.onmicrosoft.com";
        public static string SPOPassword = "lkapp2017!";
        public static string SPUrl = "https://Fjalestad2016.sharepoint.com/LK";
        public static string SPListViewTitle = "Alle elementer";

        public static ListItemCollection GetListItems(string listName)
        {
            using (ClientContext context = new ClientContext(SPUrl))
            {
                context.AuthenticationMode = ClientAuthenticationMode.Default;
                var password = new SecureString();
                foreach (char c in SPOPassword)
                {
                    password.AppendChar(c);
                }
                context.Credentials = new SharePointOnlineCredentials(SPAccount, password);

                Web web = context.Web;
                context.Load(web);
                context.Load(web, w => w.Lists);
                context.ExecuteQuery();

                List eventsList = web.Lists.GetByTitle(listName);
                context.Load(eventsList);
                context.ExecuteQuery();

                View view = eventsList.Views.GetByTitle(SPListViewTitle);
                context.Load(view);
                context.ExecuteQuery();

                CamlQuery query2 = new CamlQuery();
                query2.ViewXml = view.ViewQuery;

                ListItemCollection items = eventsList.GetItems(query2);
                context.Load(items);
                context.ExecuteQuery();

                return items;
            }        
        }
    }
}