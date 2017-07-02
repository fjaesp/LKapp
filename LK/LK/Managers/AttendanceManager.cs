using LK.Models;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Connectivity;

namespace LK.Managers
{
    public class AttendanceManager
    {
        static AttendanceManager defaultInstance = new AttendanceManager();
        MobileServiceClient client;
        IMobileServiceSyncTable<AttendEntities> attendanceTable;


        public ObservableCollection<AttendEntities> users { get; set; }

        private AttendanceManager()
        {
            client = new MobileServiceClient(Constants.ApplicationURL);
            var store = new MobileServiceSQLiteStore("LKLocal.db");
            store.DefineTable<AttendEntities>();
            client.SyncContext.InitializeAsync(store);
            attendanceTable = client.GetSyncTable<AttendEntities>();
        }

        public static AttendanceManager DefaultManager
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
            get { return attendanceTable is IMobileServiceSyncTable<AttendEntities>; }
        }

        public async Task AddCurrentUserAsAttendant(string userId, string eventId)
        {
            var attend = new AttendEntities
            {
                eventid = eventId,
                userid = userId,
            };

            await attendanceTable.InsertAsync(attend);
            await SyncAsync();
        }

        public async Task<List<EventEntities>> UpdateEventsWithAttendance(List<EventEntities> events, bool syncItems)
        {
            try
            {
                if (syncItems)
                {
                    await this.SyncAsync();
                }

                ObservableCollection<AttendEntities> attEvents = await attendanceTable
                                        .Where(a => a.userid == App.AuthResult.User.UniqueId && a.deleted == false)
                                        .ToCollectionAsync();

                if (attEvents != null && attEvents.Count > 0)
                {
                    foreach (var a in attEvents)
                    {
                        foreach (var e in events)
                        {
                            if (a.eventid == e.Id)
                                e.CurrentUserAttend = true;
                        }
                    }
                }

                return events;
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

        public async Task RemoveCurrentUserAsAttendant(string userId, string eventId)
        {

            IEnumerable<AttendEntities> attendEnum = 
                await attendanceTable.Where(x => 
                                            x.userid == userId && 
                                            x.eventid == eventId)
                                            .ToEnumerableAsync();
            int ant = attendEnum.Count();
            if(ant > 0)
            {
                await attendanceTable.DeleteAsync(attendEnum.First());
                await SyncAsync();
            }
        }

        public async Task<ObservableCollection<AttendEntities>> GetUserAttendanceAsync(string userId, bool syncItems = false)
        {
            try
            {
                if (syncItems)
                {
                    await this.SyncAsync();
                }

                IEnumerable<AttendEntities> attendEnum = 
                    await attendanceTable
                                .Where(x => x.userid == userId)
                                .ToEnumerableAsync();

                users = new ObservableCollection<AttendEntities>(attendEnum);

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

        public async Task SyncAsync()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
				ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

				try
				{
					await this.attendanceTable.PullAsync("allAttendance", attendanceTable.CreateQuery());

					await this.client.SyncContext.PushAsync();
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

        public async Task PurgeAttendTableAsync()
        {
            await attendanceTable.PurgeAsync();
        }

        //public async Task<bool> DoesCurrentUserAttend(string userId, string eventId)
        //{
        //    IEnumerable<AttendEntities> attendEnum =
        //        await attendanceTable.Where(x =>
        //                                    x.userid == userId &&
        //                                    x.eventid == eventId)
        //                                    .ToEnumerableAsync();

        //    int ant = attendEnum.Count();

        //    return ant > 0 ? true : false;
        //}

        //public async Task<ObservableCollection<AttendEntities>> GetAttendanceAsync(string userId, string eventId, bool syncItems = false)
        //{
        //    try
        //    {
        //        if (syncItems)
        //        {
        //            await this.SyncAsync();
        //        }

        //        IEnumerable<AttendEntities> attendEnum = await attendanceTable.Where(x => x.userid == userId && x.eventid == eventId).ToEnumerableAsync();
        //        users = new ObservableCollection<AttendEntities>(attendEnum);
        //        return users;
        //    }
        //    catch (MobileServiceInvalidOperationException msioe)
        //    {
        //        Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.WriteLine(@"Sync error: {0}", e.Message);
        //    }
        //    return null;
        //}

        //public async Task<ObservableCollection<AttendEntities>> GetAllEventsCurrentUserAttend(string userId)
        //{
        //    ObservableCollection<AttendEntities> result = null;

        //    IEnumerable<AttendEntities> attendEnum =
        //        await attendanceTable.Where(x =>
        //                                    x.userid == userId)
        //                                    .ToEnumerableAsync();
        //    if (attendEnum.Count() > 0)
        //        result = new ObservableCollection<AttendEntities>(attendEnum);

        //    return result;
        //}
    }
}
