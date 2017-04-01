using Microsoft.Identity.Client;
using Xamarin.Forms;

namespace LK.Views
{
    public partial class BasePage : TabbedPage
    {
        public BasePage()
        {
            InitializeComponent();

			if (Device.OS == TargetPlatform.Android)
			{
				BarBackgroundColor = Color.Transparent;
				BarTextColor = Color.Silver;
			}

            var newsNavigationPage = new NavigationPage(new NewsPage()) {
                Title = "Nyheter",
                Icon = "News25.png" //"TabPicNews.jpeg" //png"
            };
			if (Device.OS == TargetPlatform.Android)
			{ 
				newsNavigationPage.BarBackgroundColor = Color.Transparent;
				newsNavigationPage.BarTextColor = Color.Silver;
			}
            this.Children.Add(newsNavigationPage);

            var eventsNavigationPage = new NavigationPage(new EventsPage()) {
                Title = "Kalender",
				Icon = "Calendar25.png" //TabPicCalendar.jpeg"//png"
            };
			if (Device.OS == TargetPlatform.Android)
			{
				eventsNavigationPage.BarBackgroundColor = Color.Transparent;
				eventsNavigationPage.BarTextColor = Color.Silver;
			}
            this.Children.Add(eventsNavigationPage);

            var myNavigationPage = new NavigationPage(new MyPage()) {
                Title = "Min side",
                Icon = "Contacts25.png" //TabPicProfile.jpeg" //png"
            };
			if (Device.OS == TargetPlatform.Android)
			{
				myNavigationPage.BarBackgroundColor = Color.Transparent;
				myNavigationPage.BarTextColor = Color.Silver;
			}
            this.Children.Add(myNavigationPage);
        }
    }
}