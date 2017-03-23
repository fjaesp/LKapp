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
    public class CommentManager
    {
        static CommentManager defaultInstance = new CommentManager();
        //static UserManager userManager;
        MobileServiceClient client;
        IMobileServiceSyncTable<Comments> commentTable;

        public ObservableCollection<Comments> comments { get; set; }

        private CommentManager()
        {
            client = new MobileServiceClient(Constants.ApplicationURL);
            var store = new MobileServiceSQLiteStore("LKLocal.db");
            store.DefineTable<Comments>();
            client.SyncContext.InitializeAsync(store);
            commentTable = client.GetSyncTable<Comments>();
            //userManager = UserManager.DefaultManager;
        }

        public static CommentManager DefaultManager
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
            get { return commentTable is IMobileServiceSyncTable<Comments>; }
        }

        public async Task<ObservableCollection<Comments>> GetCommentsAsync(string eventId, bool syncItems = false)
        {
            try
            {
                //if (syncItems)
                //{
                //    await this.SyncAsync();
                //}

                IEnumerable<Comments> eventComments =
                    await commentTable
                                .Where(e => e.eventid == eventId)
                                .OrderByDescending(e => e.createdAt)
                                .ToEnumerableAsync();

                return new ObservableCollection<Comments>(eventComments);
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

        public async Task AddComment(string userId, string eventId, string message)
        {
            //string uName = await userManager.GetUserNameAsync(userId);
            var newComment = new Comments
            {
                eventid = eventId,
                userid = userId,
                comment = message
                //userName = string.IsNullOrEmpty(uName) ? "" : uName
            };

            await commentTable.InsertAsync(newComment);
            await SyncAsync();
        }

        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await client.SyncContext.PushAsync();
                await this.commentTable.PullAsync("allComments", commentTable.CreateQuery());
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
