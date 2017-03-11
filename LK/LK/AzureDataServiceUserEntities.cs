using LK.Models;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LK
{
    public class AzureDataServiceUserEntities
    {
        public MobileServiceClient MobileService { get; set; }
        public object CrossConnectivity { get; private set; }

        IMobileServiceSyncTable<UserEntities> userTable;

        public async Task Initialize()
        {
            if (MobileService?.SyncContext?.IsInitialized ?? false)
                return;

            //Create our client
            MobileService = new MobileServiceClient("http://lkmobileapp.azurewebsites.net");

            string path = "syncuserstore.db";
            path = Path.Combine(MobileServiceClient.DefaultDatabasePath, path);



            //setup our local sqlite store and intialize our table
            var store = new MobileServiceSQLiteStore(path);
            store.DefineTable<UserEntities>();
            await MobileService.SyncContext.InitializeAsync(store); //, new MobileServiceSyncHandler());

            //Get our sync table that will call out to azure
            userTable = MobileService.GetSyncTable<UserEntities>();
        }

        public async Task<IEnumerable> GetUser(string userName)
        {
            await Initialize();
            await SyncUser();
            return await userTable.Where(u => u.displayName.ToLower().Equals(userName.ToLower())).ToEnumerableAsync();
        }

        public async Task<IEnumerable> GetUsers()
        {
            await Initialize();
            await SyncUser();
            return await userTable.OrderBy(c => c.UpdatedAt).ToEnumerableAsync();
        }

        public async Task SyncUser()
        {
            try
            {
                await userTable.PullAsync("allUsers", userTable.CreateQuery());
                await MobileService.SyncContext.PushAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to sync users:" + ex);
            }
        }
    }
}
