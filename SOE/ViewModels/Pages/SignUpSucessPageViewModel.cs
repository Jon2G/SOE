using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using Kit.Model;
using SOE.Data;
using SOE.Views.Pages;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SOE.ViewModels.Pages
{
    public class SignUpSucessPageViewModel : ModelBase
    {

        private bool _NeedsCaptcha;
        public bool NeedsCaptcha
        {
            get => _NeedsCaptcha;
            set
            {
                _NeedsCaptcha = value;
                OnPropertyChanged();
            }
        }
        private string _Captcha;
        public string Captcha
        {
            get => _Captcha;
            set
            {
                _Captcha = value;
                OnPropertyChanged();
                this.ContinueCommand.RaiseCanExecuteChanged();
            }
        }

        private ImageSource _CaptchaImg;
        public ImageSource CaptchaImg
        {
            get => _CaptchaImg;
            set
            {
                _CaptchaImg = value;
                OnPropertyChanged();
            }
        }

        private ICommand _RefreshCaptchaCommand;
        public ICommand RefreshCaptchaCommand => _RefreshCaptchaCommand ??= new AsyncCommand(RefreshCaptcha);
        private AsyncCommand _ContinueCommand;
        public AsyncCommand ContinueCommand => _ContinueCommand ??= new AsyncCommand(Continue, ContinueCanExecute);
        private bool ContinueCanExecute(object obj) => !string.IsNullOrEmpty(Captcha);
        public bool? IsOnline { get; set; }

        public SignUpSucessPageViewModel()
        {

        }
        private async Task Continue()
        {
            if (!await AppData.Instance.SAES.LogIn(Captcha, 0, true))
            {
                App.Current.MainPage.DisplayAlert("Alerta", "El captcha es incorrecto", "Ok").SafeFireAndForget();
            }
            else
            {
                NeedsCaptcha = false;
                await GetUserData();
            }
        }
        public async Task GetUserData()
        {
            await AppData.Instance.SAES.GoTo(AppData.Instance.User.School.HomePage);
            if (await AppData.Instance.SAES.IsLoggedIn())
            {
                await AppData.Instance.SAES.GetUserData();
                await AppData.Instance.User.Save();
                Application.Current.MainPage = new WalkthroughPage();
            }
            else
            {
                RefreshCaptcha().SafeFireAndForget();
                NeedsCaptcha = true;
            }
        }

        public async Task RefreshCaptcha()
        {
            this.CaptchaImg = await AppData.Instance.SAES.GetCaptcha();
            if (CaptchaImg is null)
            {
                NeedsCaptcha = false;
                await Task.Delay(100);
                await this.GetUserData();
            }
        }


    }
}
