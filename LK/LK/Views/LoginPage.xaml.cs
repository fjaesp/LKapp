using Microsoft.Identity.Client;
using System;
using Xamarin.Forms;

namespace LK.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        //protected override async void OnAppearing()
        //{
        //    try
        //    {
        //        AuthenticationResult ar = await App.AuthenticationClient.AcquireTokenSilentAsync(
        //            Constants.Scopes,
        //            string.Empty, 
        //            Constants.Authority, 
        //            Constants.SignUpSignInpolicy,
        //            false);
        //        await Navigation.PushModalAsync(new BasePage(ar));
        //    }
        //    catch {}
        //}

        async void OnSignUpSignIn(object sender, EventArgs e)
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
                    Constants.SignUpSignInpolicy);
                await Navigation.PushModalAsync(new BasePage(ar));
            }
            catch (MsalException ee)
            {
                if (ee.Message != null && ee.Message.Contains("AADB2C90118"))
                {
                    OnForgotPassword();
                }

                if (ee.ErrorCode != "authentication_canceled")
                {
                    await DisplayAlert("An error has occurred", "Exception message: " + ee.Message, "Dismiss");
                }
            }
        }

        async void OnForgotPassword()
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
                    Constants.ResetPasswordpolicy);
                await Navigation.PushModalAsync(new BasePage(ar));
            }
            catch (MsalException ee)
            {
                if (ee.ErrorCode != "authentication_canceled")
                {
                    await DisplayAlert("An error has occurred", "Exception message: " + ee.Message, "Dismiss");
                }
            }
        }
    }
}