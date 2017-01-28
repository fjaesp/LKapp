using Microsoft.SharePoint.Client;
using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace LKapp.WebJob
{
    class Program
    {
        static void Main(string[] args)
        {
            ListItemCollection items = ReadListItems.GetListItems("Arrangementskalender");

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "lksqlserver.database.windows.net";
            builder.UserID = "lkadmin";
            builder.Password = "lkapp2017!";
            builder.InitialCatalog = "lksqldb";

            using (SQLContext context = new SQLContext(builder.ConnectionString))
            {
                Console.WriteLine("Created database schema from C# classes.");

                foreach (ListItem item in items)
                {
                    try
                    {
                        var hyperLink = (FieldUrlValue)item["Bilde"];

                        EventEntity newEvent = new EventEntity()
                        {
                            Id = item["UniqueId"].ToString(),
                            Title = item["Title"].ToString(),
                            Description = item["Beskrivelse"].ToString(),
                            Address = item["Adresse"].ToString(),
                            PictureUrl = hyperLink.Url,
                            Date = (DateTime)item["Dato"]
                        };
                        
                        if (context.Events.Any(e => e.Id == newEvent.Id))
                        {
                            context.Events.Attach(newEvent);
                            context.Entry(newEvent).State = EntityState.Modified;
                            context.SaveChanges();
                            Console.WriteLine("\nUpdated Event: " + newEvent.ToString());
                        }
                        else
                        {
                            context.Events.Add(newEvent);
                            context.SaveChanges();
                            Console.WriteLine("\nCreated Event: " + newEvent.ToString());
                        }
                    }
                    catch (SqlException e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }
        }
    }
}

