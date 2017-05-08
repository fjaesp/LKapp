using LK.Models;
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

namespace LK.Managers
{
    public class NotificationManager
    {
        static NotificationManager defaultInstance = new NotificationManager();
        MobileServiceClient client;
        IMobileServiceSyncTable<NotificationEntities> notificationTable;
        public ObservableCollection<NotificationEntities> notificaitons { get; set; }

        public NotificationManager()
        {
            client = new MobileServiceClient(Constants.ApplicationURL);
            var store = new MobileServiceSQLiteStore("LKLocal.db");
            store.DefineTable<NotificationEntities>();
            client.SyncContext.InitializeAsync(store);
            notificationTable = client.GetSyncTable<NotificationEntities>();
        }

        public static NotificationManager DefaultManager
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
            get { return notificationTable is IMobileServiceSyncTable<NotificationEntities>; }
        }

        public async Task<ObservableCollection<NotificationEntities>> GetNotificationsByUserAsync(string userId, bool syncItems = false)
        {
            try
            {
                if (syncItems)
                {
                    await this.SyncAsync();
                }

                IEnumerable<NotificationEntities> notifications = await notificationTable
                                .Where(n => n.userid == userId)
                                .OrderByDescending(e => e.createdAt)
                                .ToEnumerableAsync();

                return new ObservableCollection<NotificationEntities>(notifications);
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

        public async Task<ObservableCollection<NotificationEntities>> GetNotificationsByEventAsync(string eventId, bool syncItems = false)
        {
            try
            {
                if (syncItems)
                {
                    await this.SyncAsync();
                }

                IEnumerable<NotificationEntities> notifications = await notificationTable
                                .Where(n => n.eventid == eventId)
                                .OrderByDescending(e => e.createdAt)
                                .ToEnumerableAsync();

                return new ObservableCollection<NotificationEntities>(notifications);
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

        public async Task AddNotification(string userId, string eventId, string msg, string type)
        {
            var notification = new NotificationEntities
            {
                eventid = eventId,
                userid = userId,
                notificationtype = type,
                message = msg,
            };

            await notificationTable.InsertAsync(notification);
            await SyncAsync();
        }

        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await client.SyncContext.PushAsync();
                await this.notificationTable.PullAsync("allNotifications", notificationTable.CreateQuery());
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

        public async Task PurgeNotificationTableAsync()
        {
            await notificationTable.PurgeAsync();
        }
    }
}
