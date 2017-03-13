using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using LK.Models;
using LK.Managers;

namespace LK.Views
{
    public partial class EventPage : ContentPage
    {
        AttendanceManager attendanceManager;
        EventEntities currentEvent;

        public EventPage(EventEntities e)
        {
            InitializeComponent();
            currentEvent = e;
            BindingContext = currentEvent;

            // Check if current user is attending the event
            attendanceManager = AttendanceManager.DefaultManager;

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
            if (App.AuthResult != null)
            {
                bool doesAttend = false;
                doesAttend = await attendanceManager.DoesCurrentUserAttend(App.AuthResult.User.UniqueId, currentEvent.Id);
                if (doesAttend)
                {
                    currentEvent.currentUserAttend = true;
                    AttendSwitch.IsToggled = true;
                }
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
            if(AttendSwitch.IsToggled && !currentEvent.currentUserAttend)
            {
                await attendanceManager.AddCurrentUserAsAttendant(App.AuthResult.User.UniqueId, currentEvent.Id);
            }
            else if(AttendSwitch.IsToggled == false)
            {
                await attendanceManager.RemoveCurrentUserAsAttendant(App.AuthResult.User.UniqueId, currentEvent.Id);
            }
            
        }
    }
}
