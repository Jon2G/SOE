using SchoolOrganizer.Services;
using SchoolOrganizer.Views.Pages;
using SchoolOrganizer.Views.ViewItems;
using Xamarin.Forms;

namespace SchoolOrganizer
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
           // MainPage = new ContentPage() { Content = new UserProfile() };
            MainPage = new TaskPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
