using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace LK.Views
{
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
                await Navigation.PushModalAsync(new BasePage(ar));
            }
            catch
            {
                await Navigation.PushModalAsync(new LoginPage());
            }
        }
    }
}
