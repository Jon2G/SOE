using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using OrganizadorEscolar.Services;
using OrganizadorEscolar.Views;

namespace OrganizadorEscolar
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new SplashScreen();
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
