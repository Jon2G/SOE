using Kit.Forms.Pages;
using SOE.Views.Pages;
using Xamarin.Forms;

namespace SOE
{
    public partial class App 
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
            base.OnSleep();
        }


        protected override void OnResume()
        {
        }
    }
}
