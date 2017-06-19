using LK.Managers;
using LK.Models;
using Microsoft.Identity.Client;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Plugin.Media;
using Xamarin.Forms;

namespace LK.Views
{
    public partial class MyPage : ContentPage
    {
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
        async void TakePictureButton_Clicked(object sender, EventArgs e)
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
                    Name = "test.jpg"
                });

                if (file == null)
                    return;

                ProfilePicture.Source = ImageSource.FromStream(() => file.GetStream());


				//public static readonly string BlobStorageConnectionString = ConfigurationManager.AppSettings["BlobStorageConnectionString"];
				//CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("BlobStorageConnectionString"));
				//CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
				//CloudBlobContainer container = blobClient.GetContainerReference(blobStorageContainerName);
				//container.CreateIfNotExists();

	      //      // Retrieve reference to a blob named with passed in blobName
	      //      CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName + "|" + file.Name);
			    //ICloudBlob iCloudBlob = null;
	            //try
	            //{
             //       iCloudBlob = container.GetBlobReferenceFromServer(blobName + "|" + file.Name);
             //   }   
             //   catch (Exception e)
	            //{
	            //    Console.WriteLine(e.ToString());
	            //}

	            //file.OpenBinaryStream();
	            //ClientResult<Stream> stream = file.OpenBinaryStream();
	            //context.ExecuteQuery();

	            //stream.Value.Position = 0;
	            //blockBlob.UploadFromStream(stream.Value);


            }
			catch (MsalException ee)
			{
			}
		}

		async void UploadPictureButton_Clicked(object sender, EventArgs e)
		{
			try
			{
                if(!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await DisplayAlert("No upload", "Picking a photo is not supported.", "OK");
                    return;
                }
                var file = await CrossMedia.Current.PickPhotoAsync();

                if (file == null)
                    return;

                ProfilePicture.Source = ImageSource.FromStream(() => file.GetStream());
			}
			catch (MsalException ee)
			{
			}
		}

        private void OnAttachmentTapped(object sender, EventArgs e)
        {
            string url = ((StackLayout)sender).FindByName<Label>("AttachmentUrlString").Text;
            if(!string.IsNullOrEmpty(url))
                Device.OpenUri(new Uri(url));
        }
    }
}
