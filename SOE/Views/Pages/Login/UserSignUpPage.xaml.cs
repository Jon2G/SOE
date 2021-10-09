using AsyncAwaitBestPractices;
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

        public UserSignUpPage() : this(true)
        {

        }
        public UserSignUpPage(bool DisplayPrivacyAlert)
        {
            InitializeComponent();
            this.Model = new UserSignUpPageViewModel(this.FirstForm, this.SecondForm);
            this.BindingContext = this.Model;
            AppData.Instance.SAES = this.SAES;
            AppData.Instance.SAES.ShowLoading = false;
            this.Model.PrivacyAlertDisplayed = !DisplayPrivacyAlert;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (AppData.Instance.User.School is null)
            {
                this.Model.SelectSchoolCommand.ExecuteAsync().SafeFireAndForget();
                return;
            }
            if (!this.Model.PrivacyAlertDisplayed)
            {
                SAESPrivacyAlert.Display().SafeFireAndForget();
                this.Model.PrivacyAlertDisplayed = true;
            }
            this.Init().SafeFireAndForget();
            Usuario.Focus();
        }

        private async Task Init()
        {
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