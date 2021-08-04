using System.Windows.Input;
using AsyncAwaitBestPractices;
using Kit.Model;
using SOE.Data;
using SOE.Views.Pages;
using System.Threading.Tasks;
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
                this.ContinueCommand.ChangeCanExecute();
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
        public ICommand RefreshCaptchaCommand => _RefreshCaptchaCommand ??= new Command(RefreshCaptcha);
        private Command _ContinueCommand;
        public Command ContinueCommand => _ContinueCommand ??= new Command(Continue, ContinueCanExecute);

        private bool ContinueCanExecute() => !string.IsNullOrEmpty(Captcha);

        private async void Continue()
        {
            if (!await AppData.Instance.SAES.LogIn(Captcha,0,true))
            {
                App.Current.MainPage.DisplayAlert("Alerta","El captcha es incorrecto","Ok").SafeFireAndForget();
            }
            else
            {
                NeedsCaptcha = false;
                GetUserData();
            }
        }
        public async void GetUserData()
        {
            await AppData.Instance.SAES.GoTo(AppData.Instance.User.School.HomePage);
            if (await AppData.Instance.SAES.IsLoggedIn())
            {
                await AppData.Instance.SAES.GetUserData(AppData.Instance.User);
                AppData.Instance.User.Save();

                Application.Current.MainPage = new WalkthroughPage();
            }
            else
            {
                RefreshCaptcha();
                NeedsCaptcha = true;
            }
        }

        public async void RefreshCaptcha()
        {
            this.CaptchaImg = await AppData.Instance.SAES.GetCaptcha();
            if (CaptchaImg is null)
            {
                NeedsCaptcha = false;
                await Task.Delay(100);
                this.GetUserData();
            }
        }

        public SignUpSucessPageViewModel()
        {

        }

    }
}
