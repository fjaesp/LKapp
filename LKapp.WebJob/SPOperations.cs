using Microsoft.SharePoint.Client;
using System.Security;

namespace LKapp.WebJob
{
    public class ReadListItems
    {
        public static ListItemCollection GetListItems(string listName, string SPAccount, string SPOPassword, string SPUrl, string SPListViewTitle)
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