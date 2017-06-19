using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Foundation;
using UIKit; using Microsoft.Identity.Client;
using Microsoft.WindowsAzure.MobileServices;
using UserNotifications;
using WindowsAzure.Messaging;
using ImageCircle.Forms.Plugin.iOS;

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
            ImageCircleRenderer.Init();
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

			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) 
			{
				UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
				UIApplication.SharedApplication.RegisterForRemoteNotifications();
			}
			// IOS 10 Request notification permissions from the user
			else if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
			{
				UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert, (approved, err) =>
				{
					// Handle approval
				});
			}
			else
			{
         		UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
				UIApplication.SharedApplication.RegisterForRemoteNotificationTypes (notificationTypes);
     		}

			// Process any potential notification data from launch
 			ProcessNotification(options, true);

			// Register for Notifications
			UIApplication.SharedApplication.RegisterForRemoteNotificationTypes (
		        UIRemoteNotificationType.Alert |
		        UIRemoteNotificationType.Badge |
		        UIRemoteNotificationType.Sound);
            
			// reset our badge
			UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;

			return result;
        }

		public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
		{
			//// Register our info with Azure
			////var hub = new SBNotificationHub("<connection string with full access>", "<hub name>");
			//NSSet tags = new NSSet(new string[] { "username_" + "test123"});//Managers.NotificationManager.DefaultManager.CurrentClient.InstallationId });

			//// Connection string from your azure dashboard
			//var cs = SBConnectionString.CreateListenAccess(
			//new NSUrl(),
			//	);

			//// Register our info with Azure
			//var hub = new SBNotificationHub(cs, );
			//hub.RegisterNativeAsync ((NSData)deviceToken, tags, err => {
			//        if (err != null)
			//            Console.WriteLine("Error: " + err.Description);
			//        else
			//            Console.WriteLine("Success");
			//    });

			//string[] tags = null;
			//tags[0] = "username_";
			//tags[1] = "test123";

			RegisterForPushTags("sb://lknamespace.servicebus.windows.net/",
								"9+KonoaJ+WRQIRUH2x/w15JoCAbL849tcV3OGjMmaDI=",
								"lknotificationhub", deviceToken);//, tags);

			//const string templateBodyAPNS = "{\"aps\":{\"alert\":\"$(messageParam)\"}}";

			//JObject templates = new JObject();
			//templates["genericMessage"] = new JObject
			//	 {
			//	   {"body", templateBodyAPNS}
			//	 };

			//// Register for push with your mobile app
			//Push push = Managers.NotificationManager.DefaultManager.CurrentClient.GetPush();
			//push.RegisterAsync(deviceToken, templates);

			//NSSet tags = new NSSet(new string[] { "username_" + Managers.NotificationManager.DefaultManager.CurrentClient.InstallationId });

			//push.RegisterAsync(deviceToken, tags);
		}

		public void RegisterForPushTags(string url, string key, string hubname, object deviceToken)//, string[] tags)
		{
			var cs = SBConnectionString.CreateListenAccess(new NSUrl(url), key);

			var hub = new SBNotificationHub(cs, hubname);

			NSSet tags = new NSSet(new string[] { Managers.NotificationManager.DefaultManager.CurrentClient.InstallationId });
			//NSSet tags = new NSSet(new string[] { "$installationId:{" + Managers.NotificationManager.DefaultManager.CurrentClient.InstallationId + "}" });


			hub.RegisterNativeAsync((NSData)deviceToken, tags, err => {
			        if (err != null)
			            Console.WriteLine("Error: " + err.Description);
			        else
			            Console.WriteLine("Success");
			    });

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

		public override void ReceivedRemoteNotification(UIApplication app, NSDictionary userInfo)
		{
			// Process a notification received while the app was already open
			ProcessNotification(userInfo, false);
		}

		void ProcessNotification(NSDictionary options, bool fromFinishedLaunching)
		{
			// Check to see if the dictionary has the aps key.  This is the notification payload you would have sent
			if (null != options && options.ContainsKey(new NSString("aps")))
			{
				//Get the aps dictionary
				NSDictionary aps = options.ObjectForKey(new NSString("aps")) as NSDictionary;

				string alert = string.Empty;

				//Extract the alert text
				// NOTE: If you're using the simple alert by just specifying
				// "  aps:{alert:"alert msg here"}  ", this will work fine.
				// But if you're using a complex alert with Localization keys, etc.,
				// your "alert" object from the aps dictionary will be another NSDictionary.
				// Basically the JSON gets dumped right into a NSDictionary,
				// so keep that in mind.
				if (aps.ContainsKey(new NSString("alert")))
					alert = (aps[new NSString("alert")] as NSString).ToString();

				//If this came from the ReceivedRemoteNotification while the app was running,
				// we of course need to manually process things like the sound, badge, and alert.
				if (!fromFinishedLaunching)
				{
					//Manually show an alert
					if (!string.IsNullOrEmpty(alert))
					{
						UIAlertView avAlert = new UIAlertView("Notification", alert, null, "OK", null);
						avAlert.Show();
					}
				}
			}
 		}
    }
}