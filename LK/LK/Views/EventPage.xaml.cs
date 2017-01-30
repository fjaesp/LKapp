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
        //ViewModels.EventViewModel viewModel;
        //public double longditude;
        //public double latitude;
        //public string _Address="Addresse"; 
        //public string _Title = "Tittel"; 
        //public string _PictureUrl = ""; 
        //public string _AttachmentUrl="";
        //public DateTime _Date = DateTime.Today; 
        //public string _Description = "Beskrivelse";

        public EventPage(EventEntities e)
        {
            InitializeComponent();
            BindingContext = e;
            string test = BindingContext.ToString();


       
            //_Address = e.Address;
            //_Title = e.Title;
            //_PictureUrl = e.PictureUrl;
            //_AttachmentUrl = e.AttachmentUrl;
            //_Date = e.Date;
            //_Description = e.Description;
        //viewModel = new ViewModels.EventViewModel(e, this);
        //BindingContext = viewModel;// = new ViewModels.EventViewModel(e, this);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //var longd = getLocation(viewModel.SelectedEvent.Address, "longditude");
            //var lati = getLocation(viewModel.SelectedEvent.Address, "latitude");
            //Position pos = new Position(latitude, longditude);
            //var pin = new Pin
            //{
            //    Type = PinType.Place,
            //    Position = pos,
            //    Label = viewModel.SelectedEvent.Title,
            //    Address = viewModel.SelectedEvent.Address
            //};
            //MyMap.Pins.Add(pin);

            //MyMap.MoveToRegion(
            //    MapSpan.FromCenterAndRadius(
            //        pos, Distance.FromMiles(.2)));
        }

        //public async Task<double> getLocation(string address, string type)
        //{
            //Position p = new Position();
            //var _address = address;
            //Geocoder geoCoder = new Geocoder();
            //IEnumerable<Position> result =
            //    await geoCoder.GetPositionsForAddressAsync(_address);
            //if (result != null)
            //{
            //    foreach (Position pos in result)
            //    {
            //        System.Diagnostics.Debug.WriteLine("Lat: {0}, Lng: {1}", pos.Latitude, pos.Longitude);
            //        p = pos;
            //        if (type == "longditude")
            //        { 
            //            longditude = pos.Longitude;
            //            return pos.Longitude;
            //        }
            //        else
            //        { 
            //            latitude = pos.Latitude;
            //            return pos.Latitude;
            //        }
            //    }
            //}
           // return 0;
        //}
    }
}
