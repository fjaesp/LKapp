﻿using LK.Models;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Connectivity;

namespace LK.Managers
{
    public class UserManager
    {
        static UserManager defaultInstance = new UserManager();
        MobileServiceClient client;
        IMobileServiceSyncTable<UserEntities> userTable;

        public ObservableCollection<UserEntities> users { get; set; }

        private UserManager()
        {
            this.client = new MobileServiceClient(Constants.ApplicationURL);
            var store = new MobileServiceSQLiteStore("LKLocal.db"); //("localstore3.db");
            store.DefineTable<UserEntities>();
            this.client.SyncContext.InitializeAsync(store);
            this.userTable = client.GetSyncTable<UserEntities>();
        }

        public static UserManager DefaultManager
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
            get { return userTable is IMobileServiceSyncTable<UserEntities>; }
        }

        public async Task<UserEntities> GetUserAsync(string userId, bool syncItems = false)
        {
            try
            {
                if (syncItems)
                {
                    await this.SyncAsync();
                }

                List<UserEntities> userEnum = await userTable.Where(x => x.Id == userId).ToListAsync();
                if(userEnum != null && userEnum.Count > 0)
                {
                    if (userEnum[0].installationid == null)
					{
						userEnum[0].installationid = client.InstallationId;
						await userTable.UpdateAsync(userEnum[0]);
						await this.SyncAsync();
					}
                    else if (!userEnum[0].installationid.Contains(client.InstallationId) && CrossConnectivity.Current.IsConnected)
                    {
                        userEnum[0].installationid = userEnum[0].installationid + "," + client.InstallationId;
						await userTable.UpdateAsync(userEnum[0]);
						await this.SyncAsync();
                    }
                    return userEnum[0];
                }

                return null;
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"Sync error: {0}", e.Message);
            }

			try
			{

				ObservableCollection<UserEntities> usersList = await userTable
						.Where(u => u.Id == App.AuthResult.User.UniqueId)
						.ToCollectionAsync();

				if (usersList != null && usersList.Count > 0)
				{
					foreach (var u in users)
					{
						u.installationid = client.InstallationId;
						await userTable.UpdateAsync(u);
					}
					await this.SyncAsync();

				}
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

        public async Task<ObservableCollection<UserEntities>> GetUserAsync2(string userId, bool syncItems = false)
        {
            try
            {
                if (syncItems)
                {
                    await this.SyncAsync();
                }

                IEnumerable<UserEntities> userEnum = await userTable.Where(x => x.Id == userId).ToEnumerableAsync();
                users = new ObservableCollection<UserEntities>(userEnum);
                return users;
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

		public async Task<bool> UpdateProfilePicture(string profilePictureUrl)
		{
			try
			{ 
				ObservableCollection<UserEntities> usersList = await userTable
		        .Where(u => u.Id == App.CurrentUser.Id)
		        .ToCollectionAsync();

				if (usersList != null && usersList.Count > 0)
				{
					foreach (var u in usersList)
					{
                        u.profilepictureurl = profilePictureUrl;
						await userTable.UpdateAsync(u);
					}
					await this.SyncAsync();

				}
			}
			catch (MobileServiceInvalidOperationException msioe)
			{
				Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
                return false;
			}
			catch (Exception e)
			{
				Debug.WriteLine(@"Sync error: {0}", e.Message);
                return false;
			}
            return true;
		}

        public async Task<ObservableCollection<UserEntities>> GetUsersAsync(bool syncItems = false)
        {
            try
            {
                if (syncItems)
                {
                    await SyncAsync();
                }
            
                IEnumerable<UserEntities> es = await userTable.Where(x => x.Id != "").ToEnumerableAsync();
                users = new ObservableCollection<UserEntities>(es);
                return users;
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

        public async Task<string> GetUserNameAsync(string userId, bool syncItems = false)
        {
            try
            {
                string uName = string.Empty;

                if (syncItems)
                {
                    await this.SyncAsync();
                }

                IEnumerable<UserEntities> userEnum = await userTable.Where(x => x.Id == userId).ToEnumerableAsync();
                users = new ObservableCollection<UserEntities>(userEnum);
                if (users.Count > 0)
                    uName = users.First<UserEntities>().displayName;

                return uName;
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
        public async Task<string> GetUserProfilePictureUrl(string userId)
        {
            try
            {
                string uProfilePicUrl = string.Empty;

                IEnumerable<UserEntities> userEnum = await userTable.Where(x => x.Id == userId).ToEnumerableAsync();
                users = new ObservableCollection<UserEntities>(userEnum);
                if (users.Count > 0)
                    uProfilePicUrl = users.First<UserEntities>().profilepictureurl;

                return uProfilePicUrl;
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
            if (CrossConnectivity.Current.IsConnected)
            {
				ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

				try
				{
					//await this.client.SyncContext.PushAsync();

					// The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
					// Use a different query name for each unique query in your program.
					await this.userTable.PullAsync("allUsers", userTable.CreateQuery());
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

        public async Task PurgeUserTableAsync()
        {
            await userTable.PurgeAsync();
        }
    }
}
