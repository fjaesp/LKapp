using LK.Models;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace LK.Views
{
    public partial class MyPage : ContentPage
    {
        AuthenticationResult authResult;
        AzureDataServiceUserEntities userEntityManager;

        public MyPage(AuthenticationResult ar)
        {
            InitializeComponent();
            authResult = ar;

            userEntityManager = new AzureDataServiceUserEntities();
            BindingContext = userEntityManager.GetUser("John-Kenneth");
        }

        protected override async void OnAppearing()
        {
            if (authResult != null)
            {
                if (authResult.User.Name != "unknown")
                {
                    messageLabel.Text = authResult.User.Name;
                }
                else
                {
                    messageLabel.Text = authResult.User.UniqueId;
                }
            }
            base.OnAppearing();
        }

        async void OnSignOutBtnClicked(object sender, EventArgs e)
        {
            App.AuthenticationClient.UserTokenCache.Clear(Constants.ClientID);
            await Navigation.PopModalAsync();
        }

        async void OnResetPwdBtnClicked(object sender, EventArgs e)
        {
            try
            {
                await App.AuthenticationClient.AcquireTokenAsync(
                    Constants.Scopes,
                    string.Empty,
                    UiOptions.SelectAccount,
                    string.Empty,
                    null,
                    Constants.Authority,
                    Constants.ResetPasswordpolicy);
            }
            catch (MsalException)
            {

            }
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
                await Navigation.PushModalAsync(new BasePage(ar));
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
    }
}
