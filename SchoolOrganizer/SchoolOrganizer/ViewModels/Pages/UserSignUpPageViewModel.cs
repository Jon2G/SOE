using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using SchoolOrganizer.Views.Pages;
using Xamarin.Forms;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using APIModels;
using APIModels.Enums;
using AsyncAwaitBestPractices;
using Kit.Model;
using Newtonsoft.Json;
using SchoolOrganizer.API;
using SchoolOrganizer.Data;
using SchoolOrganizer.Models.Data;
using SchoolOrganizer.Saes;
using SkiaSharp.Views.Forms;
using SchoolOrganizer.Views.ViewItems;

namespace SchoolOrganizer.ViewModels.Pages
{
    public class UserSignUpPageViewModel : ModelBase
    {

        private User _User;
        public User User
        {
            get => _User;
            set
            {
                _User = value;
                this.ValidateCommand?.ChangeCanExecute();

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
                this.SignUpRequestedCommand.ChangeCanExecute();
            }
        }

        public School School { get; private set; }
        public int AttemptCount { get; set; }

        public Command ValidateCommand { get; }
        public Command SignUpRequestedCommand { get; }
        public ICommand FingerCommand { get; }
        public ICommand OnSignUpSuccessCommand { get; }
        public ICommand OnValidationSuccessCommand { get; }
        public string _Captcha;
        public string Captcha
        {
            get => _Captcha;
            set
            {
                _Captcha = value;
                OnPropertyChanged();
                this.ValidateCommand.ChangeCanExecute();
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
        private readonly View SecondForm;
        private readonly View FirstForm;
        public UserSignUpPageViewModel(School School, User User, View FirstForm, View SecondForm)
        {
            this.FirstForm = FirstForm;
            this.SecondForm = SecondForm;
            this.School = School;
            this.User = User;
            this.User.PropertyChanged += User_PropertyChanged;
            this.ValidateCommand = new Command(Validate, ValidateCanExecute);
            SignUpRequestedCommand = new Command(SignUpRequested, SignUpCanExecute);
            FingerCommand = new Command(FingerClicked);
            OnSignUpSuccessCommand = new Command(SignUpSuccess);
            OnValidationSuccessCommand = new Command(OnValidationSuccess);


        }

        private async void Validate()
        {
            this.AttemptCount++;
            AppData.Instance.User = this.User;
            if (await AppData.Instance.SAES.LogIn(this.User,this.Captcha,this.AttemptCount,false))
            {
                OnValidationSuccessCommand.Execute(null);
                AppData.Instance.SAES.GetName().SafeFireAndForget();
            }
            else
            {
                this.Captcha = string.Empty;
                this.User.Password = string.Empty;
                this.CaptchaImg = await AppData.Instance.SAES.GetCaptcha();
                Acr.UserDialogs.UserDialogs.Instance.Alert("Usuario o contraseña invalidos", "Atención", "Ok");
            }
        }

        private void OnValidationSuccess()
        {
            SecondForm.IsEnabled = true;
            SecondForm.IsVisible = true;
            SecondForm.FadeTo(1, 500).SafeFireAndForget();
            FirstForm.FadeTo(0, 500).SafeFireAndForget();
            FirstForm.IsEnabled = false;

        }
        private async void SignUpRequested()
        {
            Response response = Response.Error;
            APIService apiService = new APIService();
            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Iniciando sesión..."))
            {
                response = await apiService.SignUp(User.Boleta,User.Email, Password, School.Name);
            }
            switch (response.ResponseResult)
            {
                case APIResponseResult.SHOULD_ENROLL:
                    App.Current.MainPage.Navigation.PushModalAsync(new UserSignUpPage(this.School,this.User)).SafeFireAndForget();
                    break;
                case APIResponseResult.KO:
                    App.Current.MainPage.DisplayAlert("Mensaje informativo", response.Message, "Ok").SafeFireAndForget();
                    break;
                case APIResponseResult.OK:
                    this.OnSignUpSuccessCommand.Execute(AppData.Instance.SAES);
                    break;
                case APIResponseResult.DEVICE_NOT_TRUSTED:
                    //WHO ARE YOU?!
                    break;
                case APIResponseResult.INTERNAL_ERROR:
                default:
                    App.Current.MainPage.DisplayAlert("Mensaje informativo","Algo ha salido mal,esto no ha sido tu culpa.\nPor favor intenta nuevamente","Ok").SafeFireAndForget();
                    break;
            }
        }

        private void User_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.SignUpRequestedCommand?.ChangeCanExecute();
        }

        private void SignUpSuccess()
        {
            AppShell shell = new AppShell();
            if (AppData.Instance.LiteConnection.Table<User>().Any(x => x.Boleta == this.User.Boleta))
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert($"Hemos notado que es tu primer inicio de sesión, te damos la bienvenida.", $"!Bienvenido,{AppData.Instance.User.Name}¡", "Vamos allá");
            }
            AppData.Instance.LiteConnection.InsertOrReplace(this.User);
            Application.Current.MainPage = shell;
        }

        private bool SignUpCanExecute()
        {
            return !string.IsNullOrEmpty(this.Password) && !string.IsNullOrEmpty(User.Email)&&Password.Length>=8;
        }
        private bool ValidateCanExecute()
        {
            return !string.IsNullOrEmpty(User.Boleta) && Validations.IsValidBoleta(User.Boleta) && !string.IsNullOrEmpty(User.Password);
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


    }
}
