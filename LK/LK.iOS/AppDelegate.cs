using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Foundation;
using UIKit; using Microsoft.Identity.Client;
using Microsoft.WindowsAzure.MobileServices;
using UserNotifications;

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

			// Register for push notifications.
			var settings = UIUserNotificationSettings.GetSettingsForTypes(
				UIUserNotificationType.Alert
				| UIUserNotificationType.Badge
				| UIUserNotificationType.Sound,
				new NSSet());

			UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
			UIApplication.SharedApplication.RegisterForRemoteNotifications();

			// IOS 10 Request notification permissions from the user
			if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
			{
				UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert, (approved, err) =>
				{
					// Handle approval
				});
			}	
			return result;
        }
		public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
		{
			const string templateBodyAPNS = "{\"aps\":{\"alert\":\"$(messageParam)\"}}";

			JObject templates = new JObject();
			templates["genericMessage"] = new JObject
				 {
				   {"body", templateBodyAPNS}
				 };

			// Register for push with your mobile app
			Push push = Managers.NotificationManager.DefaultManager.CurrentClient.GetPush();
			push.RegisterAsync(deviceToken, templates);
		}
		public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
		{
			NSDictionary aps = userInfo.ObjectForKey(new NSString("aps")) as NSDictionary;

			string alert = string.Empty;
			if (aps.ContainsKey(new NSString("alert")))
				alert = (aps[new NSString("alert")] as NSString).ToString();

			//show alert
			if (!string.IsNullOrEmpty(alert))
			{
				UIAlertView avAlert = new UIAlertView("Notification", alert, null, "OK", null);
				avAlert.Show();
			}
		 }
    }
}