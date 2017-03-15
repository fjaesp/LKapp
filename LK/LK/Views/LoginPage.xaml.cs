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

        //        App.AuthResult = ar;
        //        await Navigation.PushModalAsync(new BasePage(ar));
        //    }
        //    catch
        //    { }
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

                App.AuthResult = ar;
                //Navigation.InsertPageBefore(new BasePage(), this);
                //await Navigation.PopAsync();
                await Navigation.PushModalAsync(new BasePage());
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

                App.AuthResult = ar;
                //Navigation.InsertPageBefore(new BasePage(), this);
                //await Navigation.PopAsync();
                await Navigation.PushModalAsync(new BasePage());
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