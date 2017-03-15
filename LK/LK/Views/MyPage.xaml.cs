using LK.Managers;
using LK.Models;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace LK.Views
{
    public partial class MyPage : ContentPage
    {
        UserManager manager;

        public MyPage()
        {
            InitializeComponent();
            manager =  UserManager.DefaultManager;
        }

        protected override async void OnAppearing()
        {
            if (App.AuthResult != null)
            {
                await GetCurrentUser(syncItems: true);
            }
            base.OnAppearing();
        }

        private async Task GetCurrentUser(bool syncItems)
        {
            ObservableCollection<UserEntities> users = await manager.GetUserAsync(App.AuthResult.User.UniqueId, syncItems);
            if(users != null)
            {
                UserEntities user = users[0];
                nameLabel.Text = user.displayName;
            }
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
                await Navigation.PushModalAsync(new BasePage());
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
