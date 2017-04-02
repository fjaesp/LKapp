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
        public MyPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            if(App.CurrentUser != null)
                nameLabel.Text = App.CurrentUser.displayName;

            base.OnAppearing();
        }

        async void OnSignOutBtnClicked(object sender, EventArgs e)
        {
            App.AuthenticationClient.UserTokenCache.Clear(Constants.ClientID);
            App.CurrentUser = null;

            //await PurgeAllTables();
            await Navigation.PopModalAsync();
        }

        async Task PurgeAllTables()
        {
            try
            {
                EventManager eManager = EventManager.DefaultManager;
                await eManager.PurgeEventTableAsync();

                AttendanceManager aManager = AttendanceManager.DefaultManager;
                await aManager.PurgeAttendTableAsync();

                CommentManager cManager = CommentManager.DefaultManager;
                await cManager.PurgeCommentsTableAsync();

                UserManager uManager = UserManager.DefaultManager;
                await uManager.PurgeUserTableAsync();

                NotificationManager nManager = NotificationManager.DefaultManager;
                await nManager.PurgeNotificationTableAsync();
            }
            catch(Exception e)
            {
                await DisplayAlert("An error has occurred", "Exception message: " + e.Message, "Dismiss");
            }
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
