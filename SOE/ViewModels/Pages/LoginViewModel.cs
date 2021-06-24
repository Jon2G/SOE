using System;
using System.Linq;
using System.Windows.Input;
using APIModels;
using APIModels.Enums;
using AsyncAwaitBestPractices;
using Kit;
using Kit.Model;
using Newtonsoft.Json;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using SOE.API;
using SOE.Data;
using SOE.Models.Data;
using SOE.Views.Pages;
using Xamarin.Forms;

namespace SOE.ViewModels.Pages
{
    public class LoginViewModel : ModelBase
    {

        public Command<LoginViewModel> LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand FingerCommand { get; }
        public ICommand OnLoginSuccess { get; }


        public LoginViewModel()
        {
            AppData.Instance.User.PropertyChanged += User_PropertyChanged;
            LoginCommand = new Command<LoginViewModel>(LoginRequested, LoginCanExecute);
            FingerCommand = new Command(FingerClicked);
            OnLoginSuccess = new Command(LoginSuccess);
            RegisterCommand = new Command(Register);
        }

        ~LoginViewModel() => AppData.Instance.User.PropertyChanged -= User_PropertyChanged;
        private void Register() => Application.Current.MainPage.Navigation.PushModalAsync(new UserSignUpPage()).SafeFireAndForget();

        private async void LoginRequested(LoginViewModel obj)
        {
            try
            {
                Response response = Response.Error;
                APIService apiService = new APIService();
                using (Acr.UserDialogs.UserDialogs.Instance.Loading("Iniciando sesión..."))
                {
                    response = await apiService.Login(AppData.Instance.User.Boleta, AppData.Instance.User.Password);
                }

                switch (response.ResponseResult)
                {
                    case APIResponseResult.SHOULD_ENROLL:
                        App.Current.MainPage.Navigation.PushModalAsync(new UserSignUpPage()).SafeFireAndForget();
                        break;
                    case APIResponseResult.KO:
                        AppData.Instance.User.Password = string.Empty;
                        App.Current.MainPage.DisplayAlert("Mensaje informativo", response.Message, "Ok")
                            .SafeFireAndForget();
                        break;
                    case APIResponseResult.OK:
                        AppData.Instance.User = JsonConvert.DeserializeObject<User>(response.Extra, new JsonSerializerSettings()
                        {
                            CheckAdditionalContent = true
                        });
                        this.OnLoginSuccess.Execute(AppData.Instance.SAES);
                        break;
                    case APIResponseResult.DEVICE_NOT_TRUSTED:
                        //WHO ARE YOU?!
                        break;
                    case APIResponseResult.INTERNAL_ERROR:
                    default:
                        App.Current.MainPage.DisplayAlert("Mensaje informativo",
                                "Algo ha salido mal,esto no ha sido tu culpa.\nPor favor intenta nuevamente", "Ok")
                            .SafeFireAndForget();
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "At LoginRequested");
                App.Current.MainPage.DisplayAlert("Mensaje informativo",
                        "Algo ha salido mal,esto no ha sido tu culpa.\nPor favor intenta nuevamente", "Ok")
                    .SafeFireAndForget();
            }
        }

        private void User_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.LoginCommand?.ChangeCanExecute();
        }

        private void LoginSuccess() => Application.Current.MainPage = new SignUpSucessPage();

        private bool LoginCanExecute(object arg)
        {
            return !string.IsNullOrEmpty(AppData.Instance.User.Boleta) && (Validations.IsValidEmail(AppData.Instance.User.Boleta) || Validations.IsValidBoleta(AppData.Instance.User.Boleta)) && !string.IsNullOrEmpty(AppData.Instance.User.Password);
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
