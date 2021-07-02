using AsyncAwaitBestPractices;
using SOE.Data;
using SOE.Saes;
using SOE.ViewModels.Pages.Login;
using SOE.Views.PopUps;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages.Login
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
            AppData.Instance.SAES.ShowLoading = false;
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