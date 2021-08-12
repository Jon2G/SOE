using System.Windows.Input;
using SOEWeb.Shared;
using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using Kit;
using Kit.Model;
using Kit.Sql.Attributes;
using SOE.Data;
using SOE.Views.Pages.Login;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SOE.ViewModels.Pages.Login
{
    [Preserve]
    public class SAESLoginPageViewModel : ModelBase
    {
        private string _Captcha;
        public string Captcha
        {
            get => _Captcha;
            set
            {
                _Captcha = value;
                Raise(() => Captcha);
                this.SignInCommand.RaiseCanExecuteChanged();
            }
        }

        private ImageSource _CaptchaImg;
        public ImageSource CaptchaImg
        {
            get => _CaptchaImg;
            set
            {
                _CaptchaImg = value;
                Raise(() => CaptchaImg);
            }
        }

        private ICommand _RefreshCaptchaCommand;
        public ICommand RefreshCaptchaCommand => _RefreshCaptchaCommand ??= new Command(RefreshCaptcha);
        private AsyncCommand _SignInCommand;

        public AsyncCommand SignInCommand
        {
            get
            {
                if (this._SignInCommand is null)
                {
                    _SignInCommand = new AsyncCommand(SignIn, ValidateCanExecute);
                }
                return this._SignInCommand;
            }
        }

        private string _Boleta;
        public string Boleta
        {
            get => _Boleta;
            set
            {
                _Boleta = value;
                Raise(() => Boleta);
                this.SignInCommand?.RaiseCanExecuteChanged();
            }
        }

        private string _Password;
        public string Password
        {
            get => _Password;
            set
            {
                _Password = value;
                Raise(() => Password);
                this.SignInCommand?.RaiseCanExecuteChanged();
            }
        }
        public bool IsLoading { get; private set; }
        public int AttemptCount { get; set; }
        public SAESLoginPageViewModel()
        {
            this.Boleta = AppData.Instance.User.Boleta;
        }

        public async void RefreshCaptcha()
        {
            this.CaptchaImg = await AppData.Instance.SAES.GetCaptcha();
            if (this.CaptchaImg is null)
            {
                await AppData.Instance.SAES.LogOut();
                this.CaptchaImg = await AppData.Instance.SAES.GetCaptcha();
            }
        }
        private async Task SignIn()
        {
            this.AttemptCount++;
            IsLoading = true;
            try
            {
                this.SignInCommand?.RaiseCanExecuteChanged();
                AppData.Instance.User.Boleta = Boleta;
                AppData.Instance.User.Password = Password;
                if (await AppData.Instance.SAES.LogIn(this.Captcha, this.AttemptCount, false))
                {
                    AppData.Instance.SAES.GetName().SafeFireAndForget();
                    LoginSucceed();
                }
                else
                {
                    AppData.Instance.User.Boleta = string.Empty;
                    AppData.Instance.User.Password = string.Empty;
                    this.Captcha = string.Empty;
                    RefreshCaptcha();
                    Acr.UserDialogs.UserDialogs.Instance.Alert("Usuario o contraseña invalidos", "Atención", "Ok");
                }
            }
            catch (Exception e)
            {
                Kit.Log.Logger.Error(e, "SignIn");
            }
            finally
            {
                IsLoading = false;
            }


        }
        private bool ValidateCanExecute(object obj)
        {
            return !string.IsNullOrEmpty(Boleta)
                   && Validations.IsValidBoleta(Boleta)
                   && !string.IsNullOrEmpty(Password)
                   && !string.IsNullOrEmpty(Captcha)
                   && !IsLoading;
        }
        internal void LoginSucceed()
        {
            App.Current.MainPage = new RefreshDataPage();
        }
    }
}
