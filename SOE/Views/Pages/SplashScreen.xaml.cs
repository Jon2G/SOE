using System;
using System.Threading.Tasks;
using Kit;
using Kit.Forms.Extensions;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using SOE.Data;
using SOE.Models.Data;
using SOE.Views.PopUps;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SplashScreen : ContentPage
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
            InitializeComponent();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await Task.Run(() => { while (ImgLogo.IsLoading) { } });
            SetStatus("Comprobando permisos de escritura...");
            await Permisos.RequestStorage();
            AppData.Init();
            User user = User.Get();
            Settings settings = new Settings();
            if (user != null)
            {
                settings = user.GetSettings();
            }
            if (user != null)
            {
                if (settings.IsFingerPrintActive)
                {
                    if (!await CrossFingerprint.Current.IsAvailableAsync(true))
                    {
                        Acr.UserDialogs.UserDialogs.Instance.Alert("La autenticación biométrica no esta disponible  o no esta configurada.", "Atención", "OK");
                        GotoManualLogin(user, settings);
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
                        GotoManualLogin(user, settings);
                    }
                    if (authResult.Authenticated)
                    {
                        GotoApp(user, settings);
                    }
                    else
                    {
                        GotoManualLogin(user, settings);
                    }
                }
                else
                {
                    GotoApp(user, settings);
                }
            }
            else
            {
                App.Current.MainPage = new LoginPage();

            }
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
                Log.Logger.Error(ex, "Al establecer el progreso en SpashScreen");
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