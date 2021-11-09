using System.Windows.Input;
using SOEWeb.Shared;
using SOEWeb.Shared.Enums;
using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using Kit.Model;
using SOE.API;
using SOE.Data;
using SOE.Models.Data;
using SOE.Views.Pages.Login;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Device = Kit.Daemon.Devices.Device;
using Kit.Forms.Model;
using System.ComponentModel.DataAnnotations;
using Kit.Forms.ComponentDataAnnotations;
using Kit.Services.Web;
using NameGenerator;
using NameGenerator.Generators;
using SOE.Views.Pages;
using SOEWeb.Shared.Interfaces;
using System.Security.Cryptography;

namespace SOE.ViewModels.Pages.Login
{
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
                _Email = value;
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
        [Validations.Boleta(ErrorMessage = "Boleta no valida.")]
        public string Boleta
        {
            get => _Boleta;
            set
            {
                _Boleta = value;
                Raise(() => Boleta);
                ValidateProperty(value);
                this.SignInCommand.RaiseCanExecuteChanged();
            }
        }


        private readonly View SecondForm;
        private readonly View FirstForm;
        private string _Captcha;
        public string Captcha
        {
            get => _Captcha;
            set
            {
                _Captcha = value;
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
        public ICommand RefreshCaptchaCommand => _RefreshCaptchaCommand ??= new Command(RefreshCaptcha);

        private AsyncCommand _SignUpCommand;
        public AsyncCommand SignUpCommand => _SignUpCommand ??= new AsyncCommand(SignUp, (o) => SignUpCanExecute);
        private AsyncCommand _SignInCommand;
        public AsyncCommand SignInCommand => _SignInCommand ??= new AsyncCommand(SignIn, SignInCanExecute);
        public AsyncCommand SelectSchoolCommand { get; }
        public int AttemptCount { get; set; }

        public bool PrivacyAlertDisplayed { get; set; }

        public UserSignUpPageViewModel(View FirstForm, View SecondForm)
        {
            this.FirstForm = FirstForm;
            this.SecondForm = SecondForm;
            this.SelectSchoolCommand = new AsyncCommand(SelectSchool);
        }

        private Task SelectSchool() => App.Current.MainPage.Navigation.PushModalAsync(new SchoolSelector(!this.PrivacyAlertDisplayed));

        private async Task SignIn()
        {
            this.AttemptCount++;
            AppData.Instance.User.Boleta = Boleta;
            AppData.Instance.User.Password = Password;
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
                AppData.Instance.User.Password =
                AppData.Instance.User.Boleta =
                this.Captcha = string.Empty;
                this.CaptchaImg = await AppData.Instance.SAES.GetCaptcha();
                Acr.UserDialogs.UserDialogs.Instance.Alert("Usuario o contraseña invalidos", "Atención", "Ok");
            }
        }
        private bool SignInCanExecute(object obj)
        {
            return !string.IsNullOrEmpty(Boleta)
                   && SOEWeb.Shared.Validations.IsValidBoleta(Boleta)
                   && !string.IsNullOrEmpty(Password);
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
        private async Task LoginSucceed()
        {
            await Task.Yield();
            SecondForm.IsEnabled = true;
            SecondForm.IsVisible = true;
            SecondForm.FadeTo(1, 500).SafeFireAndForget();
            FirstForm.FadeTo(0, 500).SafeFireAndForget();
            FirstForm.IsEnabled = false;
            GamerTagGenerator tagGenerator = new()
            {
                SpaceCharacter = "_",
                Sex = GeneratorBase.SexTypes.All,
                GeneratorFlags = GeneratorBase.NameTypes.None
            };
            while(!SOEWeb.Shared.Validations.IsValidNickName(NickName))
            {
                NickName = tagGenerator.Generate();
            }
            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Validando información");
            await AppData.Instance.SAES.GetName();
            await AppData.Instance.SAES.GetEmail();
            this.Email = AppData.Instance.User.Email;
            Acr.UserDialogs.UserDialogs.Instance.HideLoading();

        }
        private async Task SignUp()
        {
            await Task.Yield();
            AppData.Instance.User.NickName = NickName;
            Response<int> response = Response<int>.Error;
            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Iniciando sesión..."))
            {
                AppData.Instance.User.Email = this.Email;
                response = await APIService.SignUp(UserType.REGULAR_USER,
                    new SOEWeb.Shared.Device()
                    {
                        DeviceKey = Device.Current.DeviceId,
                        Brand = Device.Current.GetDeviceBrand(),
                        Platform = Device.Current.GetDevicePlatform(),
                        Model = Device.Current.GetDeviceModel(),
                        Name = Device.Current.GetDeviceName()
                    });
                switch (response.ResponseResult)
                {
                    case APIResponseResult.OK:
                        AppData.Instance.User.Id = Convert.ToInt32(response.Extra);
                        App.Current.MainPage = new RefreshDataPage(true);
                        break;
                    case APIResponseResult.INVALID_REQUEST:
                        App.Current.MainPage.DisplayAlert("Mensaje informativo", response.Message, "Ok").SafeFireAndForget();
                        break;
                    case APIResponseResult.INTERNAL_ERROR:
                        AppData.Instance.User.Id = OfflineConstants.IdBase;
                        AppData.Instance.User.IsOffline = true;
                        App.Current.MainPage = new RefreshDataPage(true, false);
                        break;
                    default:
                        Acr.UserDialogs.UserDialogs.Instance.Alert(response.Message);
                        return;
                }
            }
        }
        private bool SignUpCanExecute =>
             !string.IsNullOrEmpty(Email) &&
             !string.IsNullOrEmpty(NickName) &&
             SOEWeb.Shared.Validations.IsValidNickName(NickName);


    }
}
