using System.Linq;
using System.Windows.Input;
using APIModels;
using APIModels.Enums;
using AsyncAwaitBestPractices;
using Kit.Model;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using SOE.API;
using SOE.Data;
using SOE.Models.Data;
using SOE.Views.Pages;
using Xamarin.Forms;
using Device = Kit.Daemon.Devices.Device;

namespace SOE.ViewModels.Pages
{
    public class UserSignUpPageViewModel : ModelBase
    {

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
        public int AttemptCount { get; set; }

        public Command ValidateCommand { get; }
        public Command SignUpRequestedCommand { get; }
        public ICommand FingerCommand { get; }
        public ICommand OnSignUpSuccessCommand { get; }
        public ICommand OnValidationSuccessCommand { get; }
        private string _Captcha;
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

        private readonly View SecondForm;
        private readonly View FirstForm;
        public UserSignUpPageViewModel(View FirstForm, View SecondForm)
        {
            this.FirstForm = FirstForm;
            this.SecondForm = SecondForm;
            AppData.Instance.User.PropertyChanged += User_PropertyChanged;
            this.ValidateCommand = new Command(Validate, ValidateCanExecute);
            SignUpRequestedCommand = new Command(SignUpRequested, SignUpCanExecute);
            FingerCommand = new Command(FingerClicked);
            OnSignUpSuccessCommand = new Command(SignUpSuccess);
            OnValidationSuccessCommand = new Command(OnValidationSuccess);


        }
        ~UserSignUpPageViewModel()
        {
            AppData.Instance.User.PropertyChanged -= User_PropertyChanged;
        }
        private async void Validate()
        {
            this.AttemptCount++;
            if (await AppData.Instance.SAES.LogIn(this.Captcha, this.AttemptCount, false))
            {
                OnValidationSuccessCommand.Execute(null);
                AppData.Instance.SAES.GetName().SafeFireAndForget();
            }
            else
            {
                this.Captcha = string.Empty;
                this.CaptchaImg = await AppData.Instance.SAES.GetCaptcha();
                Acr.UserDialogs.UserDialogs.Instance.Alert("Usuario o contraseña invalidos", "Atención", "Ok");
            }
        }

        public async void RefreshCaptcha()
        {
            this.CaptchaImg = await AppData.Instance.SAES.GetCaptcha();
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
                response = await apiService.SignUp(this.Password,UserType.REGULAR_USER,
                    new APIModels.Device()
                    {
                        DeviceKey = Device.Current.DeviceId,
                        Brand = Device.Current.GetDeviceBrand(),
                        Platform = Device.Current.GetDevicePlatform(),
                        Model = Device.Current.GetDeviceModel(),
                        Name = Device.Current.GetDeviceName()
                    });
            }
            switch (response.ResponseResult)
            {
                case APIResponseResult.OK:
                    this.OnSignUpSuccessCommand.Execute(AppData.Instance.SAES);
                    break;
                case APIResponseResult.INVALID_REQUEST:
                    App.Current.MainPage.DisplayAlert("Mensaje informativo", response.Message, "Ok").SafeFireAndForget();
                    break;
                case APIResponseResult.INTERNAL_ERROR:
                default:
                    App.Current.MainPage.DisplayAlert("Mensaje informativo", "Algo ha salido mal,esto no ha sido tu culpa.\nPor favor intenta nuevamente", "Ok").SafeFireAndForget();
                    break;
            }
        }

        private void User_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.SignUpRequestedCommand?.ChangeCanExecute();
        }

        private void SignUpSuccess()
        {
            App.Current.MainPage = new SignUpSucessPage();
        }

        private bool SignUpCanExecute()
        {
            return !string.IsNullOrEmpty(this.Password) && !string.IsNullOrEmpty(AppData.Instance.User.Email) && Password.Length >= 8;
        }
        private bool ValidateCanExecute()
        {
            return !string.IsNullOrEmpty(AppData.Instance.User.Boleta) && Validations.IsValidBoleta(AppData.Instance.User.Boleta) && !string.IsNullOrEmpty(AppData.Instance.User.Password);
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
