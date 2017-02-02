using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;
using Xamarin.Forms;

namespace LK.Views
{
    public partial class EventPage : ContentPage
    {
        public EventPage(EventEntities e)
        {
            InitializeComponent();
            BindingContext = e;
            string test = BindingContext.ToString();
            double longditude=37;
            double latitude=-127;

            try
            { 
                longditude = Double.Parse(getLocation(e.Address, "longditude").ToString());
                latitude = Double.Parse(getLocation(e.Address, "latitude").ToString());
            }
            catch
            {
            }

        Position pos = new Position(longditude, latitude);
            var pin = new Pin
            {
                Type = PinType.Place,
                Position = pos,
                Label = e.Title,
                Address = e.Address
            };
            //MyMap.Pins.Add(pin);

            //MyMap.MoveToRegion(
            //    MapSpan.FromCenterAndRadius(
            //        pos, Distance.FromMiles(.2)));
        }

        protected override void OnAppearing()
        {
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
    }
}
