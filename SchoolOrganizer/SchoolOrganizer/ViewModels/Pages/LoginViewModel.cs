using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using SchoolOrganizer.Views.Pages;
using Xamarin.Forms;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using SchoolOrganizer.Data;
using SchoolOrganizer.Models.Data;
using SchoolOrganizer.Saes;
using SkiaSharp.Views.Forms;

namespace SchoolOrganizer.ViewModels.Pages
{
    public class LoginViewModel : BaseViewModel
    {
        private User _User;
        public User User
        {
            get => _User;
            set
            {
                _User = value;
                this.LoginCommand?.ChangeCanExecute();

            }
        }

        public string _Captcha;
        public string Captcha
        {
            get => _Captcha;
            set
            {
                _Captcha = value;
                this.LoginCommand.ChangeCanExecute();
            }
        }

        public ImageSource _CaptchaImg;
        public ImageSource CaptchaImg
        {
            get => _CaptchaImg;
            set
            {
                _CaptchaImg = value;
                OnPropertyChanged();
            }
        }
        public bool IsLogedIn { get; internal set; }
        public int AttemptCount { get; set; }

        public Command<LoginViewModel> LoginCommand { get; }
        public ICommand FingerCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand OnLoginSuccess { get; }


        public LoginViewModel()
        {
            this.User = new User();
            this.User.PropertyChanged += User_PropertyChanged;
            LoginCommand = new Command<LoginViewModel>(LoginRequested, LoginCanExecute);
            FingerCommand = new Command(FingerClicked);
            RegisterCommand = new Command(ConfirmRegister);
            OnLoginSuccess = new Command(LoginSuccess);

        }

        private async void LoginRequested(LoginViewModel obj)
        {
            if (await AppData.Instance.SAES.LogIn(obj))
            {
                this.OnLoginSuccess.Execute(AppData.Instance.SAES);
            }
            else
            {
                await AppData.Instance.SAES.GoTo(AppData.Instance.SAES.School.HomePage);
                await AppData.Instance.SAES.GetCaptcha();
                Acr.UserDialogs.UserDialogs.Instance.Alert("Usuario o contraseña invalidos", "Atención", "Ok");
            }
        }

        private void User_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.LoginCommand?.ChangeCanExecute();
        }

        private void LoginSuccess()
        {
            if (AppData.Instance.LiteConnection.Table<User>().Any(x => x.Boleta == this.User.Boleta))
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert($"Hemos notado que es tu primer inicio de sesión, te damos la bienvenida.", $"!Bienvenido,{AppData.Instance.User.Name}¡", "Vamos allá");
            }
            if (User.RemeberMe)
            {
                AppData.Instance.LiteConnection.InsertOrReplace(this.User);
            }
            Application.Current.MainPage = new AppShell();
        }

        private bool LoginCanExecute(object arg)
        {
            return !string.IsNullOrEmpty(User.Boleta) && User.Boleta.Length == 10 && !string.IsNullOrEmpty(User.Password) && !string.IsNullOrEmpty(Captcha);
        }


        private async void FingerClicked(object obj)
        {
            bool isFingerprintAvailable = await CrossFingerprint.Current.IsAvailableAsync(true);
            if (!isFingerprintAvailable)
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert($"Error",
                    "La autenticacion biometrica no esta disponible  o no esta configurada.", "OK");
                return;
            }

            AuthenticationRequestConfiguration conf =
                new AuthenticationRequestConfiguration("Authentication",
                "Authenticate access to your personal data");
            conf.AllowAlternativeAuthentication = true;
            var authResult = await CrossFingerprint.Current.AuthenticateAsync(conf);
            if (authResult.Authenticated)
            {
                //Success  
                //App.Current.MainPage = new AppShell();
            }
            else
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert($"Error", "Autenticacion fallida", "OK");
            }
        }
        private void ConfirmRegister(object obj)
        {
            App.Current.MainPage = new RegisterPage();
            //App.Current.MainPage = new AboutPage();
        }


    }
}
