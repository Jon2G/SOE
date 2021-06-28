using System;
using System.Windows.Input;
using APIModels;
using APIModels.Enums;
using AsyncAwaitBestPractices;
using Kit;
using Kit.Model;
using Newtonsoft.Json;
using SOE.API;
using SOE.Data;
using SOE.Models.Data;
using SOE.Views.Pages;
using SOE.Views.Pages.Login;
using Xamarin.Forms;

namespace SOE.ViewModels.Pages.Login
{
    public class LoginViewModel : ModelBase
    {

        private string _User;
        public string User
        {
            get => _User;
            set
            {
                _User = value;
                Raise(() => User);
                this.LoginCommand?.ChangeCanExecute();
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
                this.LoginCommand?.ChangeCanExecute();
            }
        }


        private Command _LoginCommand;
        public Command LoginCommand => _LoginCommand ??= new Command(LoginRequested, LoginCanExecute);

        private ICommand _RegisterCommand;
        public ICommand RegisterCommand => _RegisterCommand = new Command(Register);

        public LoginViewModel() { }


        private void Register() => Application.Current.MainPage.Navigation.PushModalAsync(new SchoolSelector()).SafeFireAndForget();

        private async void LoginRequested()
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
                        App.Current.MainPage.DisplayAlert("Mensaje informativo", response.Message, "Ok")
                            .SafeFireAndForget();
                        break;
                    case APIResponseResult.OK:
                        AppData.Instance.User = JsonConvert.DeserializeObject<User>(response.Extra, new JsonSerializerSettings()
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

        private bool LoginCanExecute()
        {
            return !string.IsNullOrEmpty(User) && (Validations.IsValidEmail(User) || Validations.IsValidBoleta(User))
                                               && !string.IsNullOrEmpty(Password)
                                               && Password.Length >= 8;
        }


    }
}
