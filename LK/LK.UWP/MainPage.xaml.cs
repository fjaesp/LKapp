using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms.Maps;

namespace LK.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new LK.App());
            Xamarin.FormsMaps.Init("PdWdgaYTjvPi2glGT8vg~LbScu1WGZXZrsXWCQjU6JQ~AvDHcwVdyFiP1Zb0TQRKdea8UueybeJDRyGKPiVPLaferq5wg6V4Ms38rJutw287");
        }
    }
}
