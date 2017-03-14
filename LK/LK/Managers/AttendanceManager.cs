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
            this.client = new MobileServiceClient(Constants.ApplicationURL);
            var store = new MobileServiceSQLiteStore("localstore3.db");
            store.DefineTable<AttendEntities>();
            this.client.SyncContext.InitializeAsync(store);
            this.attendanceTable = client.GetSyncTable<AttendEntities>();
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

        public async Task<bool> DoesCurrentUserAttend(string userId, string eventId)
        {
            IEnumerable<AttendEntities> attendEnum = 
                await attendanceTable.Where(x => 
                                            x.userid == userId && 
                                            x.eventid == eventId)
                                            .ToEnumerableAsync();

            int ant = attendEnum.Count();

            return ant > 0 ? true : false;
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

        public async Task<ObservableCollection<AttendEntities>> GetAttendanceAsync(string userId, string eventId, bool syncItems = false)
        {
            try
            {
                if (syncItems)
                {
                    await this.SyncAsync();
                }

                IEnumerable<AttendEntities> attendEnum = await attendanceTable.Where(x => x.userid == userId && x.eventid == eventId).ToEnumerableAsync();
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
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await this.attendanceTable.PullAsync(
                    "allAttendance",
                    this.attendanceTable.CreateQuery());

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
}