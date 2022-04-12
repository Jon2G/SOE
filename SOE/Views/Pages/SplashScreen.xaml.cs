using AsyncAwaitBestPractices;
using Kit;
using Microsoft.AppCenter.Crashes;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using SOE.Data;
using SOE.Models.Data;
using SOE.Views.Pages.Login;
using SOE.Views.PopUps;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Device = Xamarin.Forms.Device;

namespace SOE.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SplashScreen
    {
        private string _Status;

        public string Status
        {
            get => _Status;
            set
            {
                _Status = value;
                OnPropertyChanged();
            }
        }

        public SplashScreen()
        {
            this.BindingContext = this;
            InitializeComponent();
        }

        private async Task Init()
        {
            User user = null;
            bool signIn = true;
            UserLocalData localData = UserLocalData.Instance;
            if (!string.IsNullOrEmpty(localData.Boleta))
            {
                user = await User.Get();
                if (!string.IsNullOrEmpty(user.Boleta))
                    signIn = false;
            }
            if (signIn)
            {
                App.Current.MainPage = new UserSignUpPage();
                return;
            }
            AppData.Instance.User.Boleta = localData.Boleta;
            user ??= await User.Get();
            if (user.Settings.IsFingerPrintActive)
            {
                if (!await CrossFingerprint.Current.IsAvailableAsync(true))
                {
                    Acr.UserDialogs.UserDialogs.Instance.Alert("La autenticación biométrica no esta disponible  o no esta configurada.", "Atención", "OK");
                    GotoManualLogin(null, user.Settings);
                    return;
                }
                var authResult = await Device.InvokeOnMainThreadAsync(() =>
                    CrossFingerprint.Current.AuthenticateAsync(
                        new AuthenticationRequestConfiguration("Bloqueo de aplicación",
                            "Inicio de sesíon por huella")
                        {
                            AllowAlternativeAuthentication = true
                        }, new System.Threading.CancellationToken(false)));

                if (authResult.Status == FingerprintAuthenticationResultStatus.Failed &&
                    authResult.ErrorMessage == "Authentication canceled")
                {
                    Log.Logger.Error("Tu telegono no jala con pin amiko Im so sorry");
                    GotoManualLogin(user, user.Settings);
                }
                if (authResult.Authenticated)
                {
                    GotoApp(user, user.Settings);
                }
                else
                {
                    GotoManualLogin(user, user.Settings);
                }
            }
            else
            {
                GotoApp(user, user.Settings);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.Init().SafeFireAndForget();
        }

        private async void GotoManualLogin(User user, Settings settings)
        {
            var a = new LoginPopUp();
            await a.LockModal()
             .ShowDialog();
            GotoApp(user, settings);
        }
        public void SetStatus(string NewStatus)
        {
            try
            {
                this.Status = NewStatus;
            }
            catch (Exception ex)
            {
                Crashes.GenerateTestCrash(); Log.Logger.Error(ex, "Al establecer el progreso en SpashScreen");
            }
        }
        private async void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            await Acr.UserDialogs.UserDialogs.Instance.AlertAsync((sender as Label).Text);
        }
        private void GotoApp(User user, Settings settings)
        {
            AppData.Instance.User = user;
            App.Current.MainPage = new AppShell();
            settings.Notifications();
        }
    }
}