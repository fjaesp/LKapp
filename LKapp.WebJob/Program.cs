using Microsoft.SharePoint.Client;
using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace LKapp.WebJob
{
    class Program
    {
        //SPO Appsettings
        public static readonly string SPOListName = ConfigurationManager.AppSettings["SPOListName"];
        public static readonly string SPOAccount = ConfigurationManager.AppSettings["SPOAccount"];
        public static readonly string SPOPassword = ConfigurationManager.AppSettings["SPOPassword"];
        public static readonly string SPOUrl = ConfigurationManager.AppSettings["SPOUrl"];
        public static readonly string SPOListViewName = ConfigurationManager.AppSettings["SPOListViewName"];

        //SQL Appsettings
        public static readonly string SQLDataSource = ConfigurationManager.AppSettings["SQLDataSource"];
        public static readonly string SQLUser = ConfigurationManager.AppSettings["SQLUser"];
        public static readonly string SQLPassword = ConfigurationManager.AppSettings["SQLPassword"];
        public static readonly string SQLCatalog = ConfigurationManager.AppSettings["SQLCatalog"];

        //Office Graph Appsettings
        private static readonly string GraphUrl = ConfigurationManager.AppSettings["GraphUrl"];
        private static readonly string ClientId = ConfigurationManager.AppSettings["ClientId"];
        private static readonly string Authority = ConfigurationManager.AppSettings["Authority"];
        private static readonly string Thumbprint = ConfigurationManager.AppSettings["Thumbprint"];

        static void Main(string[] args)
        {
            Thread.Sleep(180000);            
            if (AllSetSPOandSQL())
            {
                //Getting events from SP eventslist
                ListItemCollection items = ReadListItems.GetListItems(SPOListName, SPOAccount, SPOPassword, SPOUrl, SPOListViewName);
                //Configuring SQLBuilder
                SqlConnectionStringBuilder builder = ConfigureBuilder();
                    
                //Comparing, creating and updating listitems from SP to SQL
                SQLOperations.WriteToSQL(items, builder);
            }
            if (AllSetOfficeGraph())
            {
                OfficeGraphOperations.GetUsersFromAD(GraphUrl, ClientId, Authority, Thumbprint);
                Console.WriteLine("Ferdig!");
            }
        }

        private static SqlConnectionStringBuilder ConfigureBuilder()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = SQLDataSource;
            builder.UserID = SQLUser;
            builder.Password = SQLPassword;
            builder.InitialCatalog = SQLCatalog;
            return builder;
        }
        private static bool AllSetSPOandSQL()
        {
            if (!string.IsNullOrEmpty(SPOListName) &&
            !string.IsNullOrEmpty(SPOAccount) &&
            !string.IsNullOrEmpty(SPOPassword) &&
            !string.IsNullOrEmpty(SPOUrl) &&
            !string.IsNullOrEmpty(SPOListViewName))
            {
                if (!string.IsNullOrEmpty(SQLDataSource) &&
                !string.IsNullOrEmpty(SQLUser) &&
                !string.IsNullOrEmpty(SQLPassword) &&
                !string.IsNullOrEmpty(SQLCatalog))
                {
                    return true;
                }
                Console.WriteLine("SQL Appsettings not set");
            }
            else
            {
                Console.WriteLine("SP Appsettings not set");
            }           
            return false;
        }
        private static bool AllSetOfficeGraph()
        {
            if (!string.IsNullOrEmpty(Thumbprint) &&
                !string.IsNullOrEmpty(GraphUrl) &&
                !string.IsNullOrEmpty(ClientId) &&
                !string.IsNullOrEmpty(Authority))
            {
                return true; 
            }
            else
            {
                Console.WriteLine("Office Graph Appsettings not set");
            }
            return false;
        }
    }
}

