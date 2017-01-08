using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace LK.Views
{
    public partial class EventsPage : ContentPage
    {
        //public IPlatformParameters platformParameters { get; set; }
        AuthenticationResult authenticationResult;

        public EventsPage(AuthenticationResult result)
        {
            InitializeComponent();
            authenticationResult = result;
        }

        protected override void OnAppearing()
        {
            if (authenticationResult != null)
            {
                if (authenticationResult.User.Name != "unknown")
                {
                    messageLabel.Text = string.Format("Welcome {0}", authenticationResult.User.Name);
                }
                else
                {
                    messageLabel.Text = string.Format("UserId: {0}", authenticationResult.User.UniqueId);
                }
            }
            base.OnAppearing();
        }

        async void OnLogoutButtonClicked(object sender, EventArgs e)
        {
            App.AuthenticationClient.UserTokenCache.Clear(Constants.ClientID);
            await Navigation.PopAsync();
        }

        async void OnEditButtonClicked(object sender, EventArgs e)
        {
            try
            {
                AuthenticationResult ar = await App.AuthenticationClient.AcquireTokenAsync(
                    Constants.Scopes,
                    string.Empty,
                    UiOptions.SelectAccount,
                    string.Empty,
                    null,
                    Constants.Authority,
                    Constants.EditProfilepolicy);
                await Navigation.PushAsync(new EventsPage(ar));
            }
            catch (MsalException ee)
            {
                if (ee.Message != null && ee.Message.Contains("AADB2C90118"))
                {
                    await DisplayAlert("An error has occurred", "Exception message: " + ee.Message, "Dismiss");
                }

                if (ee.ErrorCode != "authentication_canceled")
                {
                    await DisplayAlert("An error has occurred", "Exception message: " + ee.Message, "Dismiss");
                }
            }
        }

        async void OnGraphTestButtonnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new OfficeGraphPage(authenticationResult));
        }
    }
}
