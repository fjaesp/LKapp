﻿using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LK.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SplashPage : ContentPage
    {
        public SplashPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            try
            {
                AuthenticationResult ar = await App.AuthenticationClient.AcquireTokenSilentAsync(
                    Constants.Scopes,
                    string.Empty,
                    Constants.Authority,
                    Constants.SignUpSignInpolicy,
                    false);

                App.AuthResult = ar;
                await Navigation.PushModalAsync(new BasePage());
                //Navigation.InsertPageBefore(new BasePage(), this); 
                //await Navigation.PopAsync();
            }
            catch
            {
                await Navigation.PushAsync(new LoginPage());
            }
        }
    }
}
