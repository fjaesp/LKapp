using LK.Helpers;
using LK.Managers;
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
        EventManager manager;
        AttendanceManager attendManager;

        public EventsPage()
        {
			InitializeComponent();
            manager = EventManager.DefaultManager;
            attendManager = AttendanceManager.DefaultManager;
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
				var _items = await manager.GetEventsAsync(syncItems);
                ObservableCollection<Grouping<string, EventEntities>> items = _items;

                var _attendance = await attendManager.GetAllEventsCurrentUserAttend(App.AuthResult.User.UniqueId);
                ObservableCollection<AttendEntities> attendance = _attendance;

                eventList.ItemsSource = items;
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

        public async void OnEventSelected(object sender, ItemTappedEventArgs args)
        {
            var e = args.Item as EventEntities;
            if(e == null) return;

            await Navigation.PushAsync(new EventPage(e, e.CurrentUserAttend));
            eventList.SelectedItem = null;
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
    }
}
