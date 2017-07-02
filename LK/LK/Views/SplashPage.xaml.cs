using LK.Managers;
using Microsoft.Identity.Client;
using System.Threading.Tasks;
using Plugin.Connectivity;
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
            }
            catch
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
                {
                    await DisplayAlert("Feilmelding", "Mangler tilkobling til internett!", "OK");
                    await Navigation.PushAsync(new LoginPage());
                }
            }
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
                App.CurrentUser = user;
        }
    }
}
