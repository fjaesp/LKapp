using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Xamarin.Forms;
using Microsoft.Graph;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace LK.Views
{
    public partial class OfficeGraphPage : ContentPage
    {
        private static GraphServiceClient graphClient = null;
        private MailHelper _mailHelper = new MailHelper();

        public OfficeGraphPage(AuthenticationResult result)
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            SignInSignOutBtn.Text = "connect";

            // Developer code - if you haven't registered the app yet, we warn you. 
            if (Constants.ClientID == "")
            {
                InfoText.Text = "NoClientIdMessage";
                SignInSignOutBtn.IsEnabled = false;
            }
            else
            {
                InfoText.Text = "ConnectPrompt";
                SignInSignOutBtn.IsEnabled = true;
            }

        }
        async void OnSignInSignOut(object sender, EventArgs e)
        {

            if (SignInSignOutBtn.Text == "connect")
            {

                graphClient = AuthenticationHelper.GetAuthenticatedClient();
                var currentUserObject = await graphClient.Me.Request().GetAsync();      
                InfoText.Text = "Hello, " + currentUserObject.DisplayName + ". " + "SendMailPrompt";
                MailButton.IsVisible = true;
                EmailAddressBox.IsVisible = true;
                EmailAddressBox.Text = currentUserObject.UserPrincipalName;
                SignInSignOutBtn.Text = "disconnect";
            }
            else
            {
                AuthenticationHelper.SignOut();
                InfoText.Text = "ConnectPrompt";
                SignInSignOutBtn.Text = "connect";
                MailButton.IsVisible = false;
                EmailAddressBox.Text = "";
                EmailAddressBox.IsVisible = false;
            }
        }

        private async void MailButton_Click(object sender, EventArgs e)
        {
            var mailAddress = EmailAddressBox.Text;
            try
            {
                await _mailHelper.ComposeAndSendMailAsync("MailSubject", ComposePersonalizedMail(), mailAddress);
                InfoText.Text = string.Format("SendMailSuccess", mailAddress);
            }
            catch (ServiceException exception)
            {
                InfoText.Text = "MailErrorMessage";
                throw new Exception("We could not send the message: " + exception.Error == null ? "No error message returned." : exception.Error.Message);
            }
        }

        // <summary>
        // Personalizes the email.
        // </summary>
        public static string ComposePersonalizedMail()
        {
            return String.Format("MailContents", "Til du");
        }
    }
}
