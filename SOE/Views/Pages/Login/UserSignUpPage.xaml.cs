using AsyncAwaitBestPractices;
using Kit;
using SOE.Data;
using SOE.ViewModels.Pages.Login;
using SOE.Views.PopUps;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserSignUpPage
    {
        public UserSignUpPageViewModel Model { get; set; }
        public static UserSignUpPage Instance;

        public UserSignUpPage() : this(true)
        {

        }
        public UserSignUpPage(bool DisplayPrivacyAlert)
        {
            Instance = this;
            InitializeComponent();
            if (Tools.Debugging)
            {
                ContentView.Opacity =
                    PancakeView.Opacity = 0.8;
            }
            this.Model = new UserSignUpPageViewModel();
            this.BindingContext = this.Model;
            AppData.Instance.SAES = this.SAES;
            AppData.Instance.SAES.ShowLoading = false;
            UserSignUpPageViewModel.PrivacyAlertDisplayed = !DisplayPrivacyAlert;
        }
        public void OnSchoolSelected()
        {
            if (!AppData.Instance.SAES.IsNavigating)
                Init().SafeFireAndForget();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (AppData.Instance.User.School is null)
            {
                this.Model.SelectSchoolCommand.Execute(this);
                return;
            }
            if (!UserSignUpPageViewModel.PrivacyAlertDisplayed)
            {
                SAESPrivacyAlert.Display().SafeFireAndForget();
                UserSignUpPageViewModel.PrivacyAlertDisplayed = true;
            }
            this.Init().SafeFireAndForget();
        }

        private async Task Init()
        {
            await AppData.Instance.SAES.GoHome();
            if (await SAES.IsLoggedIn())
            {
                await AppData.Instance.SAES.LogOut();
                await AppData.Instance.SAES.GoHome();
            }
            await this.Model.RefreshCaptcha();
        }
    }
}