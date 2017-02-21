using Microsoft.SharePoint.Client;
using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace LKapp.WebJob
{
    public class SQLOperations
    {
        public static void WriteToSQL(ListItemCollection items, SqlConnectionStringBuilder builder)
        {
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
                            if (context.Events.Any(e => e.Title == newEvent.Title &&
                                                    e.Description == newEvent.Description &&
                                                    e.Address == newEvent.Address &&
                                                    e.PictureUrl == newEvent.PictureUrl &&
                                                    e.Date == newEvent.Date))
                            {
                                Console.WriteLine("\nNo need to update event: " + newEvent.ToString());
                            }
                            else
                            {
                                context.Events.Attach(newEvent);
                                context.Entry(newEvent).State = EntityState.Modified;
                                context.SaveChanges();
                                Console.WriteLine("\nUpdated Event: " + newEvent.ToString());
                            }
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
