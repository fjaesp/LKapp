using LK.Managers;
using LK.Models;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LK.Views
{
    public partial class NewsPage : ContentPage
    {
        NotificationManager notificationManager;
        public NewsPage()
        {
            InitializeComponent();
            notificationManager = NotificationManager.DefaultManager;
        }

        protected override async void OnAppearing()
        {
            await RefreshItems(true, syncItems: true);
            base.OnAppearing();
        }

        private async Task RefreshItems(bool showActivityIndicator, bool syncItems)
        {
            if (App.CurrentUser != null)
            {
                using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
                {
                    var items = await notificationManager.GetNotificationsByUserAsync(App.CurrentUser.Id, syncItems);
                    if (items != null)
                    {
                        MessageStack.IsVisible = false;
                        notificationList.IsVisible = true;
                        notificationList.ItemsSource = items;
                    }
                    else
                    {
                        MessageStack.IsVisible = true;
                        notificationList.IsVisible = false;
                        MessageLabel.Text = "Ingen nye varsler.";
                    }
                }
            }
            else
            {
                MessageStack.IsVisible = true;
                notificationList.IsVisible = false;
                MessageLabel.Text = "Velkommen!\n\nTa kontakt med din veileder.";
            }
        }

        public void OnNotificationSelected(object sender, ItemTappedEventArgs args)
        {
            var e = args.Item as NotificationEntities;
            if (e == null) return;
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
    }
}
