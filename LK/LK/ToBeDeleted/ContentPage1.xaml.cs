﻿using Microsoft.Identity.Client;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace LK.Views
{
    public partial class ContentPage1 : ContentPage
    {
        //EventManager manager;
        //AuthenticationResult authResult;
        public ContentPage1(AuthenticationResult ar)
        {
            InitializeComponent();
            //manager = EventManager.DefaultManager;
            //authResult = ar;
        }

        #region To be deleted
        //protected override async void OnAppearing()
        //{
        //    base.OnAppearing();
        //    await RefreshItems(true, syncItems: true);
        //}

        //private async Task RefreshItems(bool showActivityIndicator, bool syncItems)
        //{
        //    using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
        //    {
        //        eventList.ItemsSource = await manager.GetEventsAsync(syncItems);
        //    }
        //}

        //private class ActivityIndicatorScope : IDisposable
        //{
        //    private bool showIndicator;
        //    private ActivityIndicator indicator;
        //    private Task indicatorDelay;

        //    public ActivityIndicatorScope(ActivityIndicator indicator, bool showIndicator)
        //    {
        //        this.indicator = indicator;
        //        this.showIndicator = showIndicator;

        //        if (showIndicator)
        //        {
        //            indicatorDelay = Task.Delay(2000);
        //            SetIndicatorActivity(true);
        //        }
        //        else
        //        {
        //            indicatorDelay = Task.FromResult(0);
        //        }
        //    }

        //    private void SetIndicatorActivity(bool isActive)
        //    {
        //        this.indicator.IsVisible = isActive;
        //        this.indicator.IsRunning = isActive;
        //    }

        //    public void Dispose()
        //    {
        //        if (showIndicator)
        //        {
        //            indicatorDelay.ContinueWith(t => SetIndicatorActivity(false), TaskScheduler.FromCurrentSynchronizationContext());
        //        }
        //    }          
        //}

        //public async void OnSelected(object sender, SelectedItemChangedEventArgs e)
        //{
        //    if (((ListView)sender).SelectedItem != null)
        //    {
        //        await Navigation.PushAsync(new EventPage(e.SelectedItem as EventEntities));
        //        ((ListView)sender).SelectedItem = null;
        //    }
        //}

        //public async void OnRefresh(object sender, EventArgs e)
        //{
        //    var list = (ListView)sender;
        //    Exception error = null;
        //    try
        //    {
        //        await RefreshItems(false, true);
        //    }
        //    catch (Exception ex)
        //    {
        //        error = ex;
        //    }
        //    finally
        //    {
        //        list.EndRefresh();
        //    }
        //}
        #endregion
    }
}

