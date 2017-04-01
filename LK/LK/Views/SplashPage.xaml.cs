using LK.Managers;
using Microsoft.Identity.Client;
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
        UserManager xManager;
        public SplashPage()
        {
            InitializeComponent();
            xManager = UserManager.DefaultManager;
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

                await GetCurrentUser(true);

                await Navigation.PushModalAsync(new BasePage());
                //Navigation.InsertPageBefore(new BasePage(), this); 
                //await Navigation.PopAsync();
            }
            catch
            {
                await Navigation.PushAsync(new LoginPage());
            }
        }

        private async Task GetCurrentUser(bool syncItems)
        {
            var user = await xManager.GetUserAsync(App.AuthResult.User.UniqueId, syncItems);
            if (user != null)
                App.CurrentUser = user;
        }
    }
}
