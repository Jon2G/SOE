using System;
using System.Threading.Tasks;
using APIModels;
using AsyncAwaitBestPractices;
using SkiaSharp.Views.Forms;
using SOE.Data;
using SOE.Models.Data;
using SOE.Models.SkiaSharp;
using SOE.Saes;
using SOE.ViewModels.Pages;
using SOE.ViewModels.Pages.Login;
using SOE.Views.Pages.Login;
using SOE.Views.PopUps;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserSignUpPage
    {
        public UserSignUpPageViewModel Model { get; set; }

        public UserSignUpPage()
        {
            InitializeComponent();
            this.Model = new UserSignUpPageViewModel(this.FirstForm, this.SecondForm);
            this.BindingContext = this.Model;
            AppData.Instance.SAES = this.SAES;
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (AppData.Instance.User.School is null)
            {
                App.Current.MainPage.Navigation.
                    PushModalAsync(new SchoolSelector())
                    .SafeFireAndForget();
                return;
            }

            Usuario.Focus();
            SAESPrivacyAlert alert = new SAESPrivacyAlert();
            alert.ShowDialog().SafeFireAndForget();

            await AppData.Instance.SAES.GoHome();
            if (await SAES.IsLoggedIn())
            {
                await AppData.Instance.SAES.LogOut();
                await AppData.Instance.SAES.GoHome();
            }
            this.Model.RefreshCaptcha();
        }
    }
}