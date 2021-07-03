using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using APIModels;
using AsyncAwaitBestPractices;
using Kit.Model;
using SOE.Data;
using SOE.Models.Data;
using SOE.Views.Pages;
using SOE.Views.Pages.Login;
using Xamarin.Forms;

namespace SOE.ViewModels.Pages.Login
{
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
                this.SignInCommand.ChangeCanExecute();
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
        private Command _SignInCommand;
        public Command SignInCommand => _SignInCommand ??= new Command(SignIn, ValidateCanExecute);

        private string _Boleta;
        public string Boleta
        {
            get => _Boleta;
            set
            {
                _Boleta = value;
                Raise(() => Boleta);
                this.SignInCommand?.ChangeCanExecute();
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
                this.SignInCommand?.ChangeCanExecute();
            }
        }
        public int AttemptCount { get; set; }
        public SAESLoginPageViewModel()
        {
            this.Boleta = AppData.Instance.User.Boleta;
        }

        public async void RefreshCaptcha()
        {
            this.CaptchaImg = await AppData.Instance.SAES.GetCaptcha();
        }
        private async void SignIn()
        {
            this.AttemptCount++;

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
        private bool ValidateCanExecute()
        {
            return !string.IsNullOrEmpty(Boleta)
                   && Validations.IsValidBoleta(Boleta) 
                   && !string.IsNullOrEmpty(Password)
                   &&!string.IsNullOrEmpty(Captcha);
        }
        internal void LoginSucceed()
        {
            App.Current.MainPage = new RefreshDataPage();
        }
    }
}
