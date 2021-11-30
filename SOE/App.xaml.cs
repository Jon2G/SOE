using Kit.Forms.Pages;
using SOE.Views.Pages;
using Xamarin.Forms;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
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
            AppCenter.Start("android=d5e6b661-8c0f-4deb-b3ed-d451cfae8376;" +
                            "uwp={Your UWP App secret here};" +
                            "ios=28ac7f09-0df7-456a-9025-44aebd8a70be;",
                typeof(Analytics), typeof(Crashes));
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
