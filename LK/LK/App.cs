﻿using LK.Views;
using Microsoft.Identity.Client;
using Xamarin.Forms;

namespace LK
{
    public class App : Application
    {
        public static PublicClientApplication AuthenticationClient { get; private set; }

        public App()
        {
            AuthenticationClient = new PublicClientApplication(Constants.Authority, Constants.ClientID);
            //MainPage = new NavigationPage(new LoginPage());
            MainPage = new NavigationPage(new SplashPage());
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