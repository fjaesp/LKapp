using LK.Helpers;
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
    public partial class EventsPage : ContentPage
    {
        AuthenticationResult authResult;
        EventManager manager;

        public EventsPage(AuthenticationResult ar)
        {
            InitializeComponent();
            authResult = ar;
            manager = EventManager.DefaultManager;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await RefreshItems(true, syncItems: true);
        }

        private async Task RefreshItems(bool showActivityIndicator, bool syncItems)
        {
            using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
            {
                //ObservableCollection<EventEntities> items = await manager.GetEventsAsync(syncItems);
                //eventList.ItemsSource = await manager.GetEventsAsync(syncItems);

                ObservableCollection<Grouping<string, EventEntities>> items = await manager.GetEventsAsync(syncItems);
                eventList.ItemsSource = items; // await manager.GetEventsAsync(syncItems);
            }
        }

        private class ActivityIndicatorScope : IDisposable
        {
            private bool showIndicator;
            private ActivityIndicator indicator;
            private Task indicatorDelay;

            public ActivityIndicatorScope(ActivityIndicator indicator, bool showIndicator)
            {
                this.indicator = indicator;
                this.showIndicator = showIndicator;

                if (showIndicator)
                {
                    indicatorDelay = Task.Delay(2000);
                    SetIndicatorActivity(true);
                }
                else
                {
                    indicatorDelay = Task.FromResult(0);
                }
            }

            private void SetIndicatorActivity(bool isActive)
            {
                this.indicator.IsVisible = isActive;
                this.indicator.IsRunning = isActive;
            }

            public void Dispose()
            {
                if (showIndicator)
                {
                    indicatorDelay.ContinueWith(t => SetIndicatorActivity(false), TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }

        //public async void OnSelected(object sender, SelectedItemChangedEventArgs e)
        public async void OnEventSelected(object sender, ItemTappedEventArgs args)
        {
            var e = args.Item as EventEntities;
            if(e == null) return;

            await Navigation.PushAsync(new EventPage(e));
            eventList.SelectedItem = null;
            //if (((ListView)sender).SelectedItem != null)
            //{
            //    await Navigation.PushAsync(new EventPage(e.SelectedItem as EventEntities));
            //    ((ListView)sender).SelectedItem = null;
            //}
        }


        public async void OnRefresh(object sender, EventArgs e)
        {
            var list = (ListView)sender;
            Exception error = null;
            try
            {
                await RefreshItems(false, true);
            }
            catch (Exception ex)
            {
                error = ex;
            }
            finally
            {
                list.EndRefresh();
            }
        }

        #region To be deleted
        //protected override void OnAppearing()
        //{
        //    if (authenticationResult != null)
        //    {
        //        if (authenticationResult.User.Name != "unknown")
        //        {
        //            messageLabel.Text = string.Format("Welcome {0}", authenticationResult.User.Name);
        //        }
        //        else
        //        {
        //            messageLabel.Text = string.Format("UserId: {0}", authenticationResult.User.UniqueId);
        //        }
        //    }
        //    base.OnAppearing();
        //}

        //async void OnLogoutButtonClicked(object sender, EventArgs e)
        //{
        //    App.AuthenticationClient.UserTokenCache.Clear(Constants.ClientID);
        //    await Navigation.PopAsync();
        //}

        //async void OnEditButtonClicked(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        AuthenticationResult ar = await App.AuthenticationClient.AcquireTokenAsync(
        //            Constants.Scopes,
        //            string.Empty,
        //            UiOptions.SelectAccount,
        //            string.Empty,
        //            null,
        //            Constants.Authority,
        //            Constants.EditProfilepolicy);
        //        await Navigation.PushAsync(new EventsPage(ar));
        //    }
        //    catch (MsalException ee)
        //    {
        //        if (ee.Message != null && ee.Message.Contains("AADB2C90118"))
        //        {
        //            await DisplayAlert("An error has occurred", "Exception message: " + ee.Message, "Dismiss");
        //        }

        //        if (ee.ErrorCode != "authentication_canceled")
        //        {
        //            await DisplayAlert("An error has occurred", "Exception message: " + ee.Message, "Dismiss");
        //        }
        //    }
        //}

        //async void OnGraphTestButtonnClicked(object sender, EventArgs e)
        //{
        //    //await Navigation.PushAsync(new OfficeGraphPage(authenticationResult));
        //}

        //async void OnSharePointButtonnClicked(object sender, EventArgs e)
        //{
        //    //await Navigation.PushAsync(new TabbedPage1(authenticationResult));
        //}
        #endregion
    }
}
