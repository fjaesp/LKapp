using Microsoft.Identity.Client;
using Xamarin.Forms;

namespace LK.Views
{
    public partial class BasePage : TabbedPage
    {
        public BasePage(AuthenticationResult ar)
        {
            InitializeComponent();
            this.Children.Add(new NewsPage(ar));
            var eventsNavigationPage = new NavigationPage(new EventsPage(ar)) { Title = "Events" };
            this.Children.Add(eventsNavigationPage);
            var myNavigationPage = new NavigationPage(new MyPage(ar)) { Title = "My page" };
            this.Children.Add(myNavigationPage);
        }
    }
}
