using LK.Managers;
using Xamarin.Forms;

namespace LK.Views
{
    public partial class BasePage : TabbedPage
    {
        AttendanceManager attendanceManager;
        CommentManager commentManager;
        UserManager userManager;
        EventManager eventManager;
        NotificationManager notificationManager;

        public BasePage()
        {
            InitializeComponent();

            attendanceManager = AttendanceManager.DefaultManager;
            commentManager = CommentManager.DefaultManager;
            userManager = UserManager.DefaultManager;
            notificationManager = NotificationManager.DefaultManager;
            eventManager = EventManager.DefaultManager;


            if (Device.RuntimePlatform == Device.Android)
			{
				BarBackgroundColor = Color.Transparent;
				BarTextColor = Color.Silver;
			}

            var newsNavigationPage = new NavigationPage(new NewsPage()) {
                Title = "Varsler",
                Icon = "News25.png" //"TabPicNews.jpeg" //png"
            };

            if (Device.RuntimePlatform == Device.Android)
			{ 
				newsNavigationPage.BarBackgroundColor = Color.Transparent;
				newsNavigationPage.BarTextColor = Color.Silver;
			}
            this.Children.Add(newsNavigationPage);

            var eventsNavigationPage = new NavigationPage(new EventsPage()) {
                Title = "Kalender",
				Icon = "Calendar25.png" //TabPicCalendar.jpeg"//png"
            };
            if (Device.RuntimePlatform == Device.Android)
			{
				eventsNavigationPage.BarBackgroundColor = Color.Transparent;
				eventsNavigationPage.BarTextColor = Color.Silver;
			}
            this.Children.Add(eventsNavigationPage);

            var myNavigationPage = new NavigationPage(new MyPage()) {
                Title = "Min side",
                Icon = "Contacts25.png" //TabPicProfile.jpeg" //png"
            };

            if (Device.RuntimePlatform == Device.Android)
			{
				myNavigationPage.BarBackgroundColor = Color.Transparent;
				myNavigationPage.BarTextColor = Color.Silver;
			}
            this.Children.Add(myNavigationPage);
        }
    }
}