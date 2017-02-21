using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Threading;
using System.Linq;
using LK.Helpers;
using LK.Models;

namespace LK
{
    public partial class EventManager
    {
        static EventManager defaultInstance = new EventManager();
        MobileServiceClient client;
        IMobileServiceSyncTable<EventEntities> eventTable;

        public ObservableCollection<EventEntities> events { get; set; }
        public ObservableCollection<Grouping<string, EventEntities>> eventGroups { get; set; }

        private EventManager()
        {
            this.client = new MobileServiceClient(Constants.ApplicationURL);
            var store = new MobileServiceSQLiteStore("localstore3.db");
            store.DefineTable<EventEntities>();
            this.client.SyncContext.InitializeAsync(store);
            this.eventTable = client.GetSyncTable<EventEntities>();

            //Hvis du vil slette elementene i den lokale tabellen
            //bool clear = true;
            //ClearTable(clear);

            ////Midlertidig
            //int maxEvents = 6;
            //Add(maxEvents);
        }
      
        public static EventManager DefaultManager
        {
            get
            {
                return defaultInstance;
            }
            private set
            {
                defaultInstance = value;
            }
        }

        public MobileServiceClient CurrentClient
        {
            get { return client; }
        }

        public bool IsOfflineEnabled
        {
            get { return eventTable is IMobileServiceSyncTable<EventEntities>; }
        }

        public async Task<ObservableCollection<Grouping<string, EventEntities>>> GetEventsAsync(bool syncItems = false)
        //public async Task<ObservableCollection<EventEntities>> GetEventsAsync(bool syncItems = false)
        {
            try
            {
                if (syncItems)
                {
                    await this.SyncAsync();
                }

                // Hent kun fremtidige eventer og sorter disse etter nærmeste dato først
                //IEnumerable<EventEntities> es = await eventTable.Where(x => x.Date > DateTime.Now).OrderBy(x => x.Date).ToEnumerableAsync();

                IEnumerable<EventEntities> es = await eventTable
                                        .Where(x => x.Date > DateTime.Now)
                                        .OrderBy(x => x.Date)
                                        .ToEnumerableAsync();

                var sorted = from e in es
                             orderby e.Date
                             group e by e.MonthGroupName into eventGroups
                             select new Grouping<string, EventEntities>(eventGroups.Key, eventGroups);

	            eventGroups = new ObservableCollection<Grouping<string, EventEntities>>(sorted);
	            return eventGroups;
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"Sync error: {0}", e.Message);
            }
            return null;
        }

        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await this.client.SyncContext.PushAsync();

                // The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
                // Use a different query name for each unique query in your program.
                await this.eventTable.PullAsync(
                    "allEvents3",
                    this.eventTable.CreateQuery());
            }
            catch (MobileServicePushFailedException exc)
            {
                if (exc.PushResult != null)
                {
                    syncErrors = exc.PushResult.Errors;
                }
            }

            // Simple error/conflict handling.
            if (syncErrors != null)
            {
                foreach (var error in syncErrors)
                {
                    if (error.OperationKind == MobileServiceTableOperationKind.Update && error.Result != null)
                    {
                        // Update failed, revert to server's copy
                        await error.CancelAndUpdateItemAsync(error.Result);
                    }
                    else
                    {
                        // Discard local change
                        await error.CancelAndDiscardItemAsync();
                    }
                    Debug.WriteLine(@"Error executing sync operation. Item: {0} ({1}). Operation discarded.", error.TableName, error.Item["id"]);
                }
            }
        }
        //public class Grouping<K, T> : ObservableCollection<T>
        //{
        //    public K Key { get; private set; }

        //    public Grouping(K key, IEnumerable<T> items)
        //    {
        //        Key = key;
        //        foreach (var item in items)
        //            this.Items.Add(item);
        //    }
        //}

        // Midlertidig metode for å manuelt legge til data i lokal db
        //public async Task<EventEntities> AddEvent(string eventId, string title, string desc, DateTime dt, string adr, string attUrl, string picUrl, string azVersion)
        //{
        //    var ev = new EventEntities
        //    {
        //        Id = eventId,
        //        Title = title,
        //        Description = desc,
        //        Date = dt,
        //        Address = adr,
        //        AttachmentUrl = attUrl,
        //        PictureUrl = picUrl,
        //        AzureVersion = azVersion
        //    };
        //    await eventTable.InsertAsync(ev);
        //    //await SyncAsync();
        //    return ev;
        //}

        //public async Task<int> NumberOfEvents()
        //{
        //    int c = 0;

        //    try
        //    {
        //        IEnumerable<EventEntities> items = await eventTable.ToEnumerableAsync();

        //        using (var e = items.GetEnumerator())
        //        {
        //            while (e.MoveNext())
        //                c++;
        //        }
        //    }
        //    catch (MobileServiceInvalidOperationException msioe)
        //    {
        //        Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.WriteLine(@"Sync error: {0}", e.Message);
        //    }
        //    return c;

        //}
        //private async void ClearTable(bool clear)
        //{
        //    if (clear)
        //    {
        //        try
        //        {
        //            // IEnumerable<EventEntities> items = await eventTable.ToEnumerableAsync();
        //            await this.eventTable.PurgeAsync(null, null, true, CancellationToken.None);

        //            //return new ObservableCollection<EventEntities>(items);
        //        }
        //        catch (MobileServiceInvalidOperationException msioe)
        //        {
        //            Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
        //        }
        //        catch (Exception e)
        //        {
        //            Debug.WriteLine(@"Sync error: {0}", e.Message);
        //        }
        //    }
        //}

        // Midlertidig for å manueltlegge til nye events -Legger til maks 3 events nå.
        //private async void Add(int maxEvents)
        //{
        //    int x = await NumberOfEvents();
        //    if (x < maxEvents)
        //    {
        //        for (int i = 1; i <= maxEvents; i++)
        //        {
        //            await AddEvent(
        //                    i.ToString(),
        //                    "Fornuftig tittel " + i.ToString(),
        //                    "Beskrivelse " + i.ToString(),
        //                    i == 2 | i == 4 ? DateTime.Now.AddMonths(i) : DateTime.Now.AddDays(i),
        //                    "Kroerveien 4" + i.ToString(),
        //                    "",
        //                    "http://gsimages-a.akamaihd.net/bigcityadventuresparis/big_icon.png",
        //                    "2" + i.ToString()
        //                );
        //        }
        //    }
        //}
    }
}
