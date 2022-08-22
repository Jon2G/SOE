using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using Kit;
using Kit.Enums;
using Kit.Forms.ComponentDataAnnotations;
using Kit.Forms.Model;
using SOE.Data;
using SOE.Enums;
using SOE.Models;
using SOE.Models.Data;
using SOE.Validations;
using SOE.Views.Pages;
using SOE.Views.Pages.Login;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Device = Kit.Daemon.Devices.Device;

namespace SOE.ViewModels.Pages.Login
{
    public class FreeModePageViewModel : ValidationsModelbase
    {
        private string _NickName;
        [MinLength(4, ErrorMessage = "El NickName debe tener por lo menos 4 caracteres")]
        [MaxLength(10, ErrorMessage = "El NickName no puede tener más de 10 caracteres")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "el NickName no puede quedar vacio")]
        public string NickName
        {
            get => this._NickName;
            set
            {
                this._NickName = value?.Trim();
                Raise(() => NickName);
                ValidateProperty(value);
                this.SignUpCommand.RaiseCanExecuteChanged();
            }
        }
        private string _Email;
        [EmailAddress(ErrorMessage = "El correo es invalido")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "El correo no puede quedar vacio")]
        public string Email
        {
            get => _Email;
            set
            {
                _Email = value?.Trim();
                Raise(() => Email);
                ValidateProperty(value);
                this.SignUpCommand.RaiseCanExecuteChanged();
            }
        }
        private string _Password;
        [Required(AllowEmptyStrings = false, ErrorMessage = "La contraseña no puede estar vacia")]
        [MinLength(8, ErrorMessage = "Debe ser de mínimo 8 caracterés minimo")]
        public string Password
        {
            get => _Password;
            set
            {
                _Password = value;
                Raise(() => Password);
                ValidateProperty(value);
                ValidateProperty(PasswordMatch, nameof(PasswordMatch));
                this.LogInCommand.RaiseCanExecuteChanged();
            }
        }

        public string _PasswordMatch;
        [Required(AllowEmptyStrings = false, ErrorMessage = "La confirmación no puede estar vacia")]
        [MinLength(8, ErrorMessage = "Debe ser de mínimo 8 caracterés minimo")]
        [Equals(nameof(Password), ErrorMessage = "Las contraseñas no coinciden")]
        public string PasswordMatch
        {
            get => this._PasswordMatch;
            set
            {
                this._PasswordMatch = value;
                Raise(() => PasswordMatch);
                ValidateProperty(value);
                ValidateProperty(Password, nameof(Password));
                this.LogInCommand.RaiseCanExecuteChanged();
            }
        }
        private string _Boleta;
        [BoletaValidation(ErrorMessage = "Boleta no valida.")]
        public string Boleta
        {
            get => _Boleta;
            set
            {
                _Boleta = value?.Trim();
                Raise(() => Boleta);
                ValidateProperty(value);
                this.SignInCommand.RaiseCanExecuteChanged();
            }
        }
        public School School { get; set; }
        public ICommand SelectSchoolCommand { get; }
        private AsyncCommand _SignUpCommand;
        public AsyncCommand SignUpCommand => _SignUpCommand ??= new AsyncCommand(SignUp, (o) => SignUpCanExecute);
        private AsyncCommand _SignInCommand;
        public AsyncCommand SignInCommand => _SignInCommand ??= new AsyncCommand(SignIn, SignInCanExecute);
        private AsyncCommand _LogInCommand;
        public AsyncCommand LogInCommand => _LogInCommand ??= new AsyncCommand(LogIn, LogInCanExecute);
        public int AttemptCount { get; set; }
        private ContentView _CurrentView;

        public ContentView CurrentView
        {
            get => this._CurrentView;
            set
            {
                this._CurrentView = value;
                Raise(() => CurrentView);
            }
        }
        public FreeModePageViewModel(School school, ICommand selectSchoolCommand)
        {
            this.SelectSchoolCommand = selectSchoolCommand;
            this.School = school;
            CurrentView = new FreeModeStartupView(this);
            AppData.Instance.User.Mode = UserMode.FREE;
        }


        private async Task LogIn()
        {
            this.AttemptCount++;
            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Iniciando sesión..."))
            {
                if (await User.LogIn(this.Boleta, this.Password))
                {
                    LoginSucceed().SafeFireAndForget();
                }
                else
                {
                    if (AttemptCount >= 3)
                    {
                        await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Tienes varios intentos fallidos." +
                                                                              " Si continuas es posible que tu cuenta sea suspendida.",
                            "Cuidado!", "Entiendo");
                        AppData.Instance.User = new Models.Data.User();
                        App.Current.MainPage = new SplashScreen();
                        return;
                    }

                    UserLocalData.Instance.Password =
                        AppData.Instance.User.Boleta = string.Empty;
                    Acr.UserDialogs.UserDialogs.Instance.Alert("Usuario o contraseña invalidos", "Atención", "Ok");
                }
            }
        }
        private bool LogInCanExecute(object obj)
        {
            return !string.IsNullOrEmpty(Boleta)
                   && Models.Data.Validations.IsValidBoleta(Boleta) &&
                   !string.IsNullOrEmpty(Password);
        }
        private async Task SignIn()
        {
            await Task.Yield();
            AppData.Instance.User.Boleta = Boleta;
            UserLocalData.Instance.Boleta = Boleta;
            UserLocalData.Instance.SchoolId = AppData.Instance.User.School.DocumentId;
            if (!await AppData.EnsureHasInternetAccess())
            {
                AppData.Instance.User = new Models.Data.User();
                App.Current.MainPage = new SchoolSelector(true);
                await Task.Delay(500);
                Tools.Instance.Dialogs.CustomMessageBox.ShowOK("Revise su conexión a internet e intente nuevamente",
                    "Sin conexión", "Ok", CustomMessageBoxImage.Error).SafeFireAndForget();
                return;
            }

            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Espere un momento..."))
            {
                if (await User.Exists(Boleta))
                {
                    CurrentView = new FreeLoginView(this);
                }
                else
                {
                    NickName = User.GetRandomNickName();
                    CurrentView = new FreeSignUpView(this);
                }
            }
        }
        private bool SignInCanExecute(object obj)
        {
            return !string.IsNullOrEmpty(Boleta)
                   && Models.Data.Validations.IsValidBoleta(Boleta);
        }
        private async Task SignUp()
        {
            await Task.Yield();
            AppData.Instance.User.NickName = NickName;
            AppData.Instance.User.ApplicationPassword = Password;
            AppData.Instance.User.Email = Email;
            AppData.Instance.User.Name = NickName;
            AppData.Instance.User.Career = School.Name;
            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Iniciando sesión..."))
            {
                await AppData.Instance.User.School.Save();
                User user = await AppData.Instance.User.Save();
                Models.Device? device = new SOE.Models.Device()
                {
                    DeviceKey = Kit.Daemon.Devices.Device.Current.DeviceId,
                    Brand = Kit.Daemon.Devices.Device.Current.GetDeviceBrand(),
                    Platform = Kit.Daemon.Devices.Device.Current.GetDevicePlatform(),
                    Model = Kit.Daemon.Devices.Device.Current.GetDeviceModel(),
                    Name = Device.Current.GetDeviceName(),
                    LastTimeSeen = DateTime.Now
                };
                await device.Save();
                UserLocalData localData = new UserLocalData()
                {
                    SchoolId = user.School.DocumentId,
                    Boleta = Boleta,
                    Password = Password,
                    UserKey = user.DocumentId,
                };
                localData.Save();
                await this.LoginSucceed();
            }
        }
        private bool SignUpCanExecute =>
             !string.IsNullOrEmpty(Email) &&
             !string.IsNullOrEmpty(NickName) &&
             Models.Data.Validations.IsValidNickName(NickName);

        private async Task LoginSucceed()
        {
            await Task.Yield();
            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Validando información"))
            {
                User? fireUser = await User.Get();
                if (fireUser is not null)
                {
                    AppData.Instance.User.NickName = fireUser.NickName;
                    AppData.Instance.User.Email = fireUser.Email;
                }
                NickName = AppData.Instance.User.NickName;
                Email = AppData.Instance.User.Email;
                if (string.IsNullOrEmpty(AppData.Instance.User.NickName))
                {
                    NickName = User.GetRandomNickName();
                }
                UserLocalData.Instance.UserKey = Boleta;
                UserLocalData.Instance.Save();
                App.Current.MainPage = new SplashScreen();
            }
        }
    }
}
