using System;
using System.Windows.Input;
using SOEWeb.Shared;
using SOEWeb.Shared.Enums;
using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using Kit.Model;
using Newtonsoft.Json;
using SOE.API;
using SOE.Data;
using SOE.Models.Data;
using SOE.Views.Pages;
using SOE.Views.Pages.Login;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Command = Xamarin.Forms.Command;
using Log = Kit.Log;
using System.ComponentModel.DataAnnotations;
using Kit.Forms.Model;

namespace SOE.ViewModels.Pages.Login
{
    [Preserve]
    public class LoginViewModel : ValidationsModelbase
    {

        private string _User;
       [Validations.User(ErrorMessage = "Usuario no valido")]
        public string User
        {
            get => _User;
            set
            {
                _User = value;
                Raise(() => User);
                ValidateProperty(value);
                this.LoginCommand?.RaiseCanExecuteChanged();
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
                this.LoginCommand?.RaiseCanExecuteChanged();
            }
        }


        private AsyncCommand _LoginCommand;
        public AsyncCommand LoginCommand => _LoginCommand ??= new AsyncCommand(LoginRequested, LoginCanExecute);

        private ICommand _RegisterCommand;
        public ICommand RegisterCommand => _RegisterCommand ??= new Command(Register);

        private Command _DeveloperCommand;

        public Command DeveloperCommand => _DeveloperCommand ??= new Command(Developer);

        public LoginViewModel() { }


        private void Register() => Application.Current.MainPage.Navigation.PushModalAsync(new SchoolSelector()).SafeFireAndForget();

        private async Task LoginRequested()
        {
            try
            {
                Response response = Response.Error;
                using (Acr.UserDialogs.UserDialogs.Instance.Loading("Iniciando sesión..."))
                {
                    response = await APIService.Login(User, Password);
                }

                switch (response.ResponseResult)
                {
                    case APIResponseResult.SHOULD_ENROLL:
                        App.Current.MainPage.Navigation.PushModalAsync(new UserSignUpPage()).SafeFireAndForget();
                        break;
                    case APIResponseResult.KO:
                        AppData.Instance.User.Password = string.Empty;
                        App.Current.MainPage.DisplayAlert("Mensaje informativo",
                                response.Message, "Ok")
                            .SafeFireAndForget();
                        break;
                    case APIResponseResult.OK:
                        AppData.Instance.User = JsonConvert.DeserializeObject<User>(
                            response.Extra, new JsonSerializerSettings()
                        {
                            CheckAdditionalContent = true
                        });
                        Application.Current.MainPage = new SAESLoginPage();
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

        private bool LoginCanExecute(object obj)
        {
            return !string.IsNullOrEmpty(User) && (SOEWeb.Shared.Validations.IsValidEmail(User) || SOEWeb.Shared.Validations.IsValidBoleta(User))
                                               && !string.IsNullOrEmpty(Password)
                                               && Password.Length >= 8;
        }

        private async void Developer() => await Application.Current.MainPage.Navigation.PushModalAsync(new DeveloperOptions());


    }
}
