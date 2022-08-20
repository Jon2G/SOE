using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;

using Kit.Forms.Model;
using Kit.Sql.Attributes;
using SOE.Data;
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
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;

namespace SOE.ViewModels.Pages.Login
{
    [Preserve]
    public class UserSignUpPageViewModel : ValidationsModelbase
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
                this._NickName = value;
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
        public string Password
        {
            get => _Password;
            set
            {
                _Password = value;
                Raise(() => Password);
                this.SignInCommand.RaiseCanExecuteChanged();
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
        private string _Captcha;
        public string Captcha
        {
            get => _Captcha;
            set
            {
                _Captcha = value?.Trim();
                Raise(() => Captcha);
                this.SignUpCommand.RaiseCanExecuteChanged();
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
        public ICommand RefreshCaptchaCommand => _RefreshCaptchaCommand ??= new AsyncCommand(RefreshCaptcha);
        private ICommand _SaesModeCommand;
        public ICommand SaesModeCommand => _SaesModeCommand ??= new Command(SaesMode);

        private AsyncCommand _FreeModeCommand;
        public AsyncCommand FreeModeCommand => null; //FreeModeCommand ??= new AsyncCommand(this.FreeMode);

        private AsyncCommand _SignUpCommand;
        public AsyncCommand SignUpCommand => _SignUpCommand ??= new AsyncCommand(SignUp, (o) => SignUpCanExecute);
        private AsyncCommand _SignInCommand;
        public AsyncCommand SignInCommand => _SignInCommand ??= new AsyncCommand(SignIn, SignInCanExecute);
        public ICommand SelectSchoolCommand { get; }
        public int AttemptCount { get; set; }

        public static bool PrivacyAlertDisplayed { get; set; }
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

        public UserSignUpPageViewModel()
        {
            this.SelectSchoolCommand = new Command(SelectSchool);
            SaesMode();
            //this.CurrentView = new ModeSelectorView(this);
        }

        private void SaesMode()
        {
            this.CurrentView = new SaesLoginView(this);
        }

        private Task FreeMode() => App.Current.MainPage.Navigation.PushModalAsync(new FreeModePage(new FreeModePageViewModel(AppData.Instance.User.School, this.SelectSchoolCommand)));

        private void SelectSchool()
        {
            App.Current.MainPage.Navigation.PushModalAsync(new SchoolSelector(!PrivacyAlertDisplayed)).SafeFireAndForget();
        }

        private async Task SignIn()
        {
            await Task.Yield();
            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Iniciando sesión"))
            {
                this.AttemptCount++;
                AppData.Instance.User.Boleta = Boleta;
                UserLocalData.Instance.Boleta = Boleta;
                UserLocalData.Instance.Password = Password;
                UserLocalData.Instance.SchoolId = AppData.Instance.User.School.DocumentId;
                if (await AppData.Instance.SAES.LogIn(this.Captcha, this.AttemptCount, false))
                {
                    LoginSucceed().SafeFireAndForget();
                }
                else
                {
                    if (AttemptCount >= 3)
                    {
                        await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Tienes varios intentos fallidos. Es posible que la aplicación no pueda comunicarse correctamente con el SAES o tus datos sean incorrectos." +
                                                                    " Si continuas es posible que tu cuenta sea suspendida.", "Cuidado!", "Entiendo");
                        AppData.Instance.User = new Models.Data.User();
                        App.Current.MainPage = new SplashScreen();
                        return;
                    }
                    UserLocalData.Instance.Password =
                    AppData.Instance.User.Boleta =
                    this.Captcha = string.Empty;
                    this.CaptchaImg = await AppData.Instance.SAES.GetCaptcha();
                    Acr.UserDialogs.UserDialogs.Instance.Alert("Usuario o contraseña invalidos", "Atención", "Ok");
                }
            }

        }
        private bool SignInCanExecute(object? obj)
        {
            return !string.IsNullOrEmpty(Boleta)
                   && Models.Data.Validations.IsValidBoleta(Boleta)
                   && !string.IsNullOrEmpty(Password);
        }
        public async Task RefreshCaptcha()
        {
            this.CaptchaImg = await AppData.Instance.SAES.GetCaptcha();
            if (this.CaptchaImg is null)
            {
                await AppData.Instance.SAES.LogOut();
                this.CaptchaImg = await AppData.Instance.SAES.GetCaptcha();
            }
        }
        private async Task LoginSucceed()
        {
            await Task.Yield();
            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Validando información");
            User? fireUser = await User.Get();
            if (fireUser is not null)
            {
                AppData.Instance.User.NickName = fireUser.NickName;
                AppData.Instance.User.Email = fireUser.Email;
            }
            NickName = AppData.Instance.User.NickName;
            Email = AppData.Instance.User.Email;
            if (!string.IsNullOrEmpty(NickName) && !string.IsNullOrEmpty(Email))
            {
                SignUp().SafeFireAndForget();
                return;
            }
            if (string.IsNullOrEmpty(AppData.Instance.User.NickName))
            {
                NickName = User.GetRandomNickName();
            }
            if (string.IsNullOrEmpty(AppData.Instance.User.Email))
            {
                await AppData.Instance.SAES.GetEmail();
            }
            await AppData.Instance.SAES.GetName();

            this.Email = AppData.Instance.User.Email;
            if (fireUser.Mode == Enums.UserMode.SAES)
            {
                CurrentView = new UserDataView(this);
            }
            Acr.UserDialogs.UserDialogs.Instance.HideLoading();

        }
        private async Task SignUp()
        {
            await Task.Yield();
            AppData.Instance.User.NickName = NickName;
            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Iniciando sesión..."))
            {
                await AppData.Instance.User.School.Save();
                User user = await AppData.Instance.User.Save();
                Models.Device device = new SOE.Models.Device()
                {
                    DeviceKey = Device.Current.DeviceId,
                    Brand = Device.Current.GetDeviceBrand(),
                    Platform = Device.Current.GetDevicePlatform(),
                    Model = Device.Current.GetDeviceModel(),
                    Name = Device.Current.GetDeviceName(),
                    LastTimeSeen = DateTime.Now
                };
                await device.Save();
                UserLocalData localData = new UserLocalData()
                {
                    SchoolId = user.School.DocumentId,
                    Boleta = Boleta,
                    Password = Password,
                    UserKey = user.DocumentId
                };
                localData.Save();
                App.Current.MainPage = new RefreshDataPage(true);

                //switch (response.ResponseResult)
                //{
                //    case APIResponseResult.OK:
                //        AppData.Instance.User.Id = Convert.ToInt32(response.Extra);
                //        App.Current.MainPage = new RefreshDataPage(true);
                //        break;
                //    case APIResponseResult.INVALID_REQUEST:
                //        App.Current.MainPage.DisplayAlert("Mensaje informativo", response.Message, "Ok").SafeFireAndForget();
                //        break;
                //    case APIResponseResult.INTERNAL_ERROR:
                //        AppData.Instance.User.Id = OfflineConstants.IdBase;
                //        AppData.Instance.User.IsOffline = true;
                //        App.Current.MainPage = new RefreshDataPage(true, false);
                //        break;
                //    default:
                //        Acr.UserDialogs.UserDialogs.Instance.Alert(response.Message);
                //        return;
                //}
            }
        }
        private bool SignUpCanExecute =>
             !string.IsNullOrEmpty(Email) &&
             !string.IsNullOrEmpty(NickName) &&
             Models.Data.Validations.IsValidNickName(NickName);


    }
}
