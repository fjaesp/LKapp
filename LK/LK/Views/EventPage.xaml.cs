using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using LK.Models;
using LK.Managers;
using System.Collections.ObjectModel;

namespace LK.Views
{
    public partial class EventPage : ContentPage
    {
        AttendanceManager attendanceManager;
        CommentManager commentManager;
        static UserManager userManager;
        EventEntities currentEvent;

        public string AttUrlStr { get; private set; }

        public EventPage(EventEntities e)
        {
            InitializeComponent();
            currentEvent = e;
            
            BindingContext = currentEvent;

            string[] att = string.IsNullOrEmpty(e.AttachmentUrl) ? null : e.AttachmentUrl.Split(',');
            ObservableCollection<AttachmentEntity> attList;
            if(att != null)
            {
                attList = new ObservableCollection<AttachmentEntity>();
                for (int i = 0; i < att.Length; i++)
                {
                    attList.Add(new AttachmentEntity
                    {
                        url = att[i]
                    });
                }

                AttachmentList.ItemsSource = attList;
            }

            // Check if current user is attending the event
            attendanceManager = AttendanceManager.DefaultManager;
            commentManager = CommentManager.DefaultManager;
            userManager = UserManager.DefaultManager;

            #region kart utkommentert
            //string test = BindingContext.ToString();
            //double longditude=37;
            //double latitude=-127;

            //try
            //{ 
            //    longditude = Double.Parse(getLocation(e.Address, "longditude").ToString());
            //    latitude = Double.Parse(getLocation(e.Address, "latitude").ToString());
            //}
            //catch
            //{
            //}

            //Position pos = new Position(longditude, latitude);
            //var pin = new Pin
            //{
            //    Type = PinType.Place,
            //    Position = pos,
            //    Label = e.Title,
            //    Address = e.Address
            //};
            ////MyMap.Pins.Add(pin);

            ////MyMap.MoveToRegion(
            ////    MapSpan.FromCenterAndRadius(
            ////        pos, Distance.FromMiles(.2)));
            #endregion
        }

        protected override async void OnAppearing()
        {
            await RefreshComments(true, syncItems: true);
            if(currentEvent.CurrentUserAttend)
            {
                AttendSwitch.IsToggled = true;
            }
            base.OnAppearing();
        }

        public async Task<double> getLocation(string address, string type)
        {
            Position p = new Position();
            var _address = address;
            Geocoder geoCoder = new Geocoder();
            IEnumerable<Position> result =
                await geoCoder.GetPositionsForAddressAsync(_address);
            if (result != null)
            {
                foreach (Position pos in result)
                {
                    System.Diagnostics.Debug.WriteLine("Lat: {0}, Lng: {1}", pos.Latitude, pos.Longitude);
                    p = pos;
                    if (type == "longditude")
                    {
                        return pos.Longitude;
                    }
                    else
                    {
                        return pos.Latitude;
                    }
                }
            }
            return 0;
        }

        private async void AttendSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            if(AttendSwitch.IsToggled && !currentEvent.CurrentUserAttend)
            {
                await attendanceManager.AddCurrentUserAsAttendant(App.AuthResult.User.UniqueId, currentEvent.Id);
            }
            else if(AttendSwitch.IsToggled == false)
            {
                await attendanceManager.RemoveCurrentUserAsAttendant(App.AuthResult.User.UniqueId, currentEvent.Id);
            }
            
        }

        private async Task RefreshAttendance(bool showActivityIndicator, bool syncItems)
        {
            using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
            {
                ObservableCollection<AttendEntities> items = 
                    await attendanceManager.GetUserAttendanceAsync(App.AuthResult.User.UniqueId, syncItems);

                for (int i = 0; i < items.Count; i++)
                {
                    if(items[i].eventid == currentEvent.Id)
                    {
                        currentEvent.CurrentUserAttend = true;
                        AttendSwitch.IsToggled = true;
                    }
                }
            }
        }

        private async Task RefreshComments(bool showActivityIndicator, bool syncItems)
        {
            using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
            {
                ObservableCollection<Comments> items = await commentManager.GetCommentsAsync(currentEvent.Id, syncItems);

                for(int i=0; i<items.Count; i++)
                {
                    items[i].userName = await userManager.GetUserNameAsync(items[i].userid);
                }

                CommentList.ItemsSource = items;
            }
        }

        private async Task CommentList_RefreshingAsync(object sender, EventArgs e)
        {
            var list = (ListView)sender;
            Exception error = null;
            try
            {
                await RefreshComments(false, true);
            }
            catch (Exception ex)
            {
                error = ex;
            }
            finally
            {
                list.EndRefresh();
            }
        }

        private class ActivityIndicatorScope : IDisposable
        {
            private bool showIndicator;
            private ActivityIndicator indicator;
            private Task indicatorDelay;

            public ActivityIndicatorScope(ActivityIndicator indicator, bool showIndicator)
            {
                this.indicator = indicator;
                this.showIndicator = showIndicator;

                if (showIndicator)
                {
                    indicatorDelay = Task.Delay(2000);
                    SetIndicatorActivity(true);
                }
                else
                {
                    indicatorDelay = Task.FromResult(0);
                }
            }

            private void SetIndicatorActivity(bool isActive)
            {
                this.indicator.IsVisible = isActive;
                this.indicator.IsRunning = isActive;
            }

            public void Dispose()
            {
                if (showIndicator)
                {
                    indicatorDelay.ContinueWith(t => SetIndicatorActivity(false), TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }

        private async Task CommentEntry_CompletedAsync(object sender, EventArgs e)
        {
            string newComment = ((Entry)sender).Text;
            if (newComment.Length > 0)
            {
                await commentManager.AddComment(App.AuthResult.User.UniqueId, currentEvent.Id, newComment);
                await RefreshComments(true, syncItems: true);
                //CommentList.ItemsSource = await commentManager.GetCommentsAsync(currentEvent.Id, true);

                

                ((Entry)sender).Text = "";
            }
        }

        private void OnAttachmentTapped(object sender, EventArgs e)
        { 
            Device.OpenUri(new Uri(""));
        }
    }
}
