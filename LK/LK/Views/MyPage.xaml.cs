using LK.Managers;
using LK.Models;
using Microsoft.Identity.Client;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Plugin.Media;
using Xamarin.Forms;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Plugin.Connectivity;


namespace LK.Views
{
    public partial class MyPage : ContentPage
    {
		UserManager userManager;

        public MyPage()
        {
            InitializeComponent();

            string[] att = string.IsNullOrEmpty(App.CurrentUser.AttachmentUrl) ? null : App.CurrentUser.AttachmentUrl.Split(',');
            ObservableCollection<AttachmentEntity> attList;
            if (att != null)
            {
                attList = new ObservableCollection<AttachmentEntity>();
                for (int i = 0; i < att.Length; i++)
                {
                    attList.Add(new AttachmentEntity
                    {
                        url = att[i]
                    });
                }

                AttachmentList.ItemsSource = attList;
            }

			TapGestureRecognizer imageTap = new TapGestureRecognizer();
			imageTap.Tapped += TakePictureButton_Clicked;
			ProfilePicture.GestureRecognizers.Add(imageTap);
        }

        protected override void OnAppearing()
        {
            if(App.CurrentUser != null)
            {
				nameLabel.Text = App.CurrentUser.displayName;
				RefreshProfilePicture();
            }
                
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

        void RefreshProfilePicture()
		{
			try
			{
                if (App.CurrentUser != null && CrossConnectivity.Current.IsConnected)
                {
                    if (App.CurrentUser.profilepictureurl != "")
                    {
                        ProfilePicture.Source = new UriImageSource
                        {
                            Uri = new Uri(App.CurrentUser.profilepictureurl),
                            CachingEnabled = false
                        };
                    }
                }
                else
                    ProfilePicture.Source = ImageSource.FromFile("defaultprofilepic.png");
			}
			catch (MsalException ee)
			{
			}
		}

		async void TakePictureButton_Clicked(object sender, EventArgs e)
		{
			if (CrossConnectivity.Current.IsConnected)
			{
				string userProfilePicturesContainerName = "userprofilepicturescontainer";
				string connectionString = "DefaultEndpointsProtocol=https;AccountName=lkappstorage;AccountKey=0PpAIi7FI8YRcNeg539IuFVXSDEA+EXyBEr1ykbQKgkChvUdKCnZsshIhIqKwOnSSJwwybxkx0vLH+MzTfg85Q==;";

				var action = await DisplayActionSheet("Bytt profilbilde?", "Avbryt", null, "Ta bilde", "Last opp");
                if (action == "Ta bilde")
                {
                    try
                    {
                        await CrossMedia.Current.Initialize();

                        if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                        {
                            await DisplayAlert("No Camera", "No camara available.", "OK");
                            return;
                        }

                        var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                        {
                            SaveToAlbum = true,
                            Name = App.CurrentUser.Id + ".jpg"
                        });

                        if (file == null)
                            return;

                        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
                        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                        CloudBlobContainer container = blobClient.GetContainerReference(userProfilePicturesContainerName);
                        await container.CreateIfNotExistsAsync();

                        // Retrieve reference to a blob named with passed in blobName
                        CloudBlockBlob blockBlob = container.GetBlockBlobReference(App.CurrentUser.Id + ".jpg");
                        blockBlob.Properties.ContentType = "image/jpeg";
                        await blockBlob.UploadFromStreamAsync(file.GetStream());

                        try
                        {
                            userManager = UserManager.DefaultManager;
                            bool updated = await userManager.UpdateProfilePicture("https://lkappstorage.blob.core.windows.net/userprofilepicturescontainer/" + App.CurrentUser.Id + ".jpg");
                        }
                        catch (Exception eee)
                        {
                        }

                        RefreshProfilePicture();
                    }
                    catch (MsalException ee)
                    {
                    }
                }
				if (action == "Last opp")
				{
					try
					{
						if (!CrossMedia.Current.IsPickPhotoSupported)
						{
							await DisplayAlert("No upload", "Picking a photo is not supported.", "OK");
							return;
						}
						var file = await CrossMedia.Current.PickPhotoAsync();

						if (file == null)
							return;

						CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
						CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
						CloudBlobContainer container = blobClient.GetContainerReference(userProfilePicturesContainerName);
						await container.CreateIfNotExistsAsync();

						// Retrieve reference to a blob named with passed in blobName
						CloudBlockBlob blockBlob = container.GetBlockBlobReference(App.CurrentUser.Id + ".jpg");
						blockBlob.Properties.ContentType = "image/jpeg";
						await blockBlob.UploadFromStreamAsync(file.GetStream());

						try
						{
							userManager = UserManager.DefaultManager;
							bool updated = await userManager.UpdateProfilePicture("https://lkappstorage.blob.core.windows.net/userprofilepicturescontainer/" + App.CurrentUser.Id + ".jpg");
						}
						catch (Exception eee)
						{
						}

						RefreshProfilePicture();
					}
					catch (MsalException ee)
					{
					}
				}
			}
			else
				await DisplayAlert("Feilmelding", "Mangler tilkobling til internett!", "OK");
		}

        private void OnAttachmentTapped(object sender, EventArgs e)
        {
			if (CrossConnectivity.Current.IsConnected)
			{
				string url = ((StackLayout)sender).FindByName<Label>("AttachmentUrlString").Text;
				if (!string.IsNullOrEmpty(url))
					Device.OpenUri(new Uri(url));
			}
			else
				DisplayAlert("Feilmelding", "Mangler tilkobling til internett!", "OK");
        }
    }
}
