using LK.Views;
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
        public static PublicClientApplication AuthenticationClient { get; private set; }

        public App()
        {
            AuthenticationClient = new PublicClientApplication(Constants.Authority, Constants.ClientID);
            MainPage = new ContentPage1();//new TabbedPage1();//new NavigationPage(new Views.LoginPage());
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