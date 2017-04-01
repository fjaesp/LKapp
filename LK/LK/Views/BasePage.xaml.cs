using Microsoft.Identity.Client;
using Xamarin.Forms;

namespace LK.Views
{
    public partial class BasePage : TabbedPage
    {
        public BasePage()
        {
            InitializeComponent();
            var newsNavigationPage = new NavigationPage(new NewsPage()) {
                Title = "Nyheter",
                Icon = "TabPicNews.png"
            };
            this.Children.Add(newsNavigationPage);
            var eventsNavigationPage = new NavigationPage(new EventsPage()) {
                Title = "Kalender",
                Icon = "TabPicCalendar.png"
            };
            this.Children.Add(eventsNavigationPage);
            var myNavigationPage = new NavigationPage(new MyPage()) {
                Title = "Min side",
                Icon = "TabPicProfile.png"
            };
            this.Children.Add(myNavigationPage);
        }
    }
}