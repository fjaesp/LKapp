using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace LK
{
    public class App : Application
    {
        public static PublicClientApplication PCApplication { get; set; }

        public static string ClientID = "79364928-572a-44d8-8455-ee32e9d0cd95";
        public static string[] Scopes = { ClientID };
        public static string SignUpSignInpolicy = "B2C_1_SiUpIn";
        public static string ResetPasswordpolicy = "B2C_1_SSPR";
        public static string Authority = "https://login.microsoftonline.com/LKapp.onmicrosoft.com/";

        public App()
        {
            PCApplication = new PublicClientApplication(Authority, ClientID); 
            MainPage = new NavigationPage(new LK.Views.LoginPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
