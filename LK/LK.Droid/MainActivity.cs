using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Microsoft.Identity.Client;
using Gcm.Client;
using System;
using ImageCircle.Forms.Plugin.Droid;
using Plugin.Permissions;

namespace LK.Droid
{
    [Activity(Label = "LK App", 
        Icon = "@drawable/icon", 
        Theme = "@style/MainTheme",
        MainLauncher = true, 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]

    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
			// Set the current instance of MainActivity.
 			instance = this;
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(bundle);


            // Initialize Azure Mobile Apps
			Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

			// Initialize Xamarin Forms
			global::Xamarin.Forms.Forms.Init (this, bundle);
            ImageCircleRenderer.Init();

			//Initialize Xamarin Forms Maps
            //Xamarin.FormsMaps.Init(this, bundle);

			//Load the main application
            LoadApplication(new App());

            App.AuthenticationClient.PlatformParameters = new PlatformParameters(Xamarin.Forms.Forms.Context as Activity);

			try
 			{
		     	// Check to ensure everything's set up right
		     	GcmClient.CheckDevice(this);
				GcmClient.CheckManifest(this);

				// Register for push notifications
				System.Diagnostics.Debug.WriteLine("Registering...");
				GcmClient.Register(this, PushHandlerBroadcastReceiver.SENDER_IDS);
			}
 			catch (Java.Net.MalformedURLException)
 			{
	 			CreateAndShowDialog("There was an error creating the client. Verify the URL.", "Error");
 			}
 			catch (Exception e)
 			{
	 			CreateAndShowDialog(e.Message, "Error");
 			}
        }
		// Create a new instance field for this activity.
		static MainActivity instance = null;

		// Return the current activity instance.
		public static MainActivity CurrentActivity
		{
			get
			{
				return instance;
   			}
		}
		private void CreateAndShowDialog(String message, String title)
		{
			AlertDialog.Builder builder = new AlertDialog.Builder(this);

			builder.SetMessage(message);
			builder.SetTitle(title);
			builder.Create().Show();
 		}

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationAgentContinuationHelper.SetAuthenticationAgentContinuationEventArgs(requestCode, resultCode, data);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

