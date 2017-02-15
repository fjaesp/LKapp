using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Foundation;
using UIKit; using Microsoft.Identity.Client;

namespace LK.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
			Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
			SQLitePCL.CurrentPlatform.Init();
			global::Xamarin.Forms.Forms.Init();
			LoadApplication(new App());

			var result = base.FinishedLaunching(app, options);

			var platformParameters = UIApplication.SharedApplication.KeyWindow.RootViewController;
			App.AuthenticationClient.PlatformParameters = new PlatformParameters(platformParameters);

			return result;
        }
    }
}