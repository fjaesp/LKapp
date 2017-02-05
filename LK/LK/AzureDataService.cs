using LK;
using LK.Models;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(AzureDataService))]
namespace LK
{
    public class AzureDataService
    {
        public MobileServiceClient MobileService { get; set; } = null;
        IMobileServiceSyncTable<EventEntities> eventEntities;
        //public static bool UseAuth { get; set; } = false;

        public async Task Initialize()
        {

            if (MobileService?.SyncContext?.IsInitialized ?? false)
                return;

            var appUrl = "http://lkmobileapp.azurewebsites.net";
            MobileService = new MobileServiceClient(appUrl);

            const string path = "syncstore.db";
            //setup our local sqlite store and intialize our table
            var store = new MobileServiceSQLiteStore(path);
            store.DefineTable<EventEntities>();
            await MobileService.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());

            //Get our sync table that will call out to azure
            eventEntities = MobileService.GetSyncTable<EventEntities>();
        }

        public async Task<IEnumerable> GetEvents()
        {
            await Initialize();
            await SyncEvents();
            return await eventEntities.OrderBy(c => c.Date).ToEnumerableAsync();
        }

        //public async Task AddEvent(bool madeAtHome)
        //{
        //}

        public async Task SyncEvents()
        {
            try
            {
                //pull down all latest changes and then push current coffees up
                await eventEntities.PullAsync("allEvents", eventEntities.CreateQuery());
                await MobileService.SyncContext.PushAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to sync events, that is alright as we have offline capabilities: " + ex);
            }
        }
    }
}
