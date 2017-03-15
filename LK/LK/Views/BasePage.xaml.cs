using Microsoft.Identity.Client;
using Xamarin.Forms;

namespace LK.Views
{
    public partial class BasePage : TabbedPage
    {
        public BasePage()
        {
            InitializeComponent();
            this.Children.Add(new NewsPage());
            var eventsNavigationPage = new NavigationPage(new EventsPage()) { Title = "Events" };
            this.Children.Add(eventsNavigationPage);
            var myNavigationPage = new NavigationPage(new MyPage()) { Title = "My page" };
            this.Children.Add(myNavigationPage);
        }
    }
}
