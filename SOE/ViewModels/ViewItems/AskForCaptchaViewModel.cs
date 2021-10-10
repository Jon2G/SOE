using AsyncAwaitBestPractices.MVVM;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using SOEWeb.Shared;
using Kit.Model;
using SOE.Data;
using SOE.Views.PopUps;
using Xamarin.Forms;
using Command = Xamarin.Forms.Command;

namespace SOE.ViewModels.ViewItems
{
    public class AskForCaptchaViewModel : ModelBase
    {
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
        private string _Captcha;
        public string Captcha
        {
            get => _Captcha;
            set
            {
                _Captcha = value;
                Raise(() => Captcha);
                this.SignInCommand.RaiseCanExecuteChanged();
            }
        }
        private ICommand _RefreshCaptchaCommand;
        public ICommand RefreshCaptchaCommand => _RefreshCaptchaCommand ??= new Command(RefreshCaptcha);
        private AsyncCommand _SignInCommand;
        public AsyncCommand SignInCommand => _SignInCommand ??= new AsyncCommand(SignIn, ValidateCanExecute);
        public int AttemptCount { get; set; }
        public readonly Func<AskForCaptcha, Task<bool>> OnSucceedAction;
        private readonly AskForCaptcha AskForCaptcha;
        public bool IsLoading { get; private set; }

        private bool _ShouldConfirmPassword;

        public bool ShouldConfirmPassword
        {
            get => _ShouldConfirmPassword;
            set { _ShouldConfirmPassword = value; Raise(() => ShouldConfirmPassword); }
        }

        private string _NewPasword;

        public string NewPasword
        {
            get => _NewPasword;
            set
            {
                _NewPasword = value;
                Raise(() => NewPasword);
            }
        }

        public AskForCaptchaViewModel(AskForCaptcha AskForCaptcha, Func<AskForCaptcha, Task<bool>> OnSucceedAction)
        {
            this.AskForCaptcha = AskForCaptcha;
            this.OnSucceedAction = OnSucceedAction;
        }
        public async void RefreshCaptcha()
        {
            this.CaptchaImg = await AppData.Instance.SAES.GetCaptcha();
        }
        private async Task SignIn()
        {
            await Task.Yield();
            if (!this.ValidateCanExecute(null))
            {
                return;
            }
            this.AttemptCount++;
            IsLoading = true;
            this.SignInCommand.RaiseCanExecuteChanged();
            try
            {
                if (NewPasword is { Length: > 0 })
                {
                    AppData.Instance.User.Password = NewPasword;
                    AppData.Instance.User.Save();
                }

                if (await AppData.Instance.SAES.LogIn(this.Captcha, this.AttemptCount, false))
                {
                    await OnSucceedAction.Invoke(this.AskForCaptcha);
                }
                else
                {
                    this.ShouldConfirmPassword = true;
                    this.Captcha = string.Empty;
                    RefreshCaptcha();
                    Acr.UserDialogs.UserDialogs.Instance.Alert("Usuario o contraseña invalidos", "Atención", "Ok");
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                IsLoading = false;
            }
        }
        private bool ValidateCanExecute(object o)
        {
            return !string.IsNullOrEmpty(AppData.Instance.User.Boleta)
                   && SOEWeb.Shared.Validations.IsValidBoleta(AppData.Instance.User.Boleta)
                   && !string.IsNullOrEmpty(AppData.Instance.User.Password)
                   && !string.IsNullOrEmpty(Captcha) && !IsLoading;
        }
    }
}
