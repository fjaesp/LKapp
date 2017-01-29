using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace LK
{
    public partial class EventManager
    {
        static EventManager defaultInstance = new EventManager();
        MobileServiceClient client;
        IMobileServiceSyncTable<EventEntities> eventTable;

        private EventManager()
        {
            this.client = new MobileServiceClient(Constants.ApplicationURL);
            var store = new MobileServiceSQLiteStore("localstore.db");
            store.DefineTable<EventEntities>();
            this.client.SyncContext.InitializeAsync(store);
            this.eventTable = client.GetSyncTable<EventEntities>();
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
            get { return eventTable is Microsoft.WindowsAzure.MobileServices.Sync.IMobileServiceSyncTable<EventEntities>; }
        }

        public async Task<ObservableCollection<EventEntities>> GetEventsAsync(bool syncItems = false)
        {
            try
            {
                if (syncItems)
                {
                    await this.SyncAsync();
                }
                IEnumerable<EventEntities> items = await eventTable
                    //.Where(item != item.Title)
                    .ToEnumerableAsync();

                return new ObservableCollection<EventEntities>(items);
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
                    "allEvents",
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
    }
}
