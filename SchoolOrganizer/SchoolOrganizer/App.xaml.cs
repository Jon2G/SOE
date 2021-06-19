using Kit;
using SchoolOrganizer.Data;
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
            App.Current.MainPage = new SplashScreen();

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
