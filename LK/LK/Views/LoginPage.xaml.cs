using LK.Managers;
using Microsoft.Identity.Client;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Connectivity;

namespace LK.Views
{
    public partial class LoginPage : ContentPage
    {
        UserManager xManager;
        public LoginPage()
        {
            InitializeComponent();
            xManager = UserManager.DefaultManager;
        }

        async void OnSignUpSignIn(object sender, EventArgs e)
        {
			if (CrossConnectivity.Current.IsConnected)
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

					await GetCurrentUser(true);

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
			else
				await DisplayAlert("Feilmelding", "Mangler tilkobling til internett!", "OK");			
        }

        async void OnForgotPassword()
        {
            if (CrossConnectivity.Current.IsConnected)
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
            else
                await DisplayAlert("Feilmelding", "Mangler tilkobling til internett!", "OK"); 

        }

        private async Task GetCurrentUser(bool syncItems)
        {
            var user = await xManager.GetUserAsync(App.AuthResult.User.UniqueId, syncItems);
            if (user != null)
			{
                App.CurrentUser = user;
				user.installationid = xManager.CurrentClient.InstallationId;
			}
        }
    }
}