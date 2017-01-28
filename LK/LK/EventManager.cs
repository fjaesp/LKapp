using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace LK
{
    public partial class EventManager
    {
        static EventManager defaultInstance = new EventManager();
        MobileServiceClient client;

        IMobileServiceTable<EventEntities> eventTable;
        //const string offlineDbPath = @"localstore.db";

        private EventManager()
        {
            this.client = new MobileServiceClient(Constants.ApplicationURL);
            this.eventTable = client.GetTable<EventEntities>();

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
        public async Task<ObservableCollection<EventEntities>> GetTodoItemsAsync(bool syncItems = false)
        {
            try
            {
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
    }
}
