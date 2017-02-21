using LK;
using LK.Models;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(AzureDataService))]
namespace LK
{
    public class AzureDataService
    {
        public MobileServiceClient MobileService { get; set; } = null;
        IMobileServiceSyncTable<EventEntities> eventTable;
        //public static bool UseAuth { get; set; } = false;

        public async Task Initialize()
        {

            if (MobileService?.SyncContext?.IsInitialized ?? false)
                return;

            var appUrl = "http://lkmobileapp.azurewebsites.net";

            //Create Client
            MobileService = new MobileServiceClient(appUrl);

            //InitialzeDatabase for path
            string path = "syncstore2.db";
            path = Path.Combine(MobileServiceClient.DefaultDatabasePath, path);

            //setup our local sqlite store and intialize our table
            var store = new MobileServiceSQLiteStore(path);

            //Define table
            store.DefineTable<EventEntities>();

            //Initialize SyncContext
            await MobileService.SyncContext.InitializeAsync(store);//, new MobileServiceSyncHandler());

            //Get our sync table that will call out to azure
            eventTable = MobileService.GetSyncTable<EventEntities>();
        }

        public async Task<IEnumerable> GetEvents()
        {
            //Initialize & Sync
            await Initialize();
            await SyncEvents();

            return await eventTable.OrderBy(c => c.Date).ToEnumerableAsync();
        }

        //public async Task AddEvent(bool madeAtHome)
        //{
        //}

        public async Task SyncEvents()
        {
            try
            {
                //pull down all latest changes and then push current coffees up
                await eventTable.PullAsync("allEvents2", eventTable.CreateQuery());
                await MobileService.SyncContext.PushAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to sync events, that is alright as we have offline capabilities: " + ex);
            }
        }
    }
}
