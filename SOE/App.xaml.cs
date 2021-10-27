using SOE.Views.Pages;
using Xamarin.Forms;

namespace SOE
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
