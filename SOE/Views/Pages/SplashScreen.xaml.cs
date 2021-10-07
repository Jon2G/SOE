using AsyncAwaitBestPractices;
using System;
using System.Threading.Tasks;
using Kit;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using SOE.Data;
using SOE.Models.Data;
using SOE.Views.Pages.Login;
using SOE.Views.PopUps;
using SOEWeb.Shared;
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
            AppData.Init();
            if (Tools.Debugging)
            {
                AppData.CreateDatabase();
            }
            User user = null;
            if (AppData.Instance.LiteConnection.TableExists<User>())
            {
                user = User.Get();
            }
            else
            {
                AppData.Instance.LiteConnection.CreateTable<User>();
                AppData.Instance.LiteConnection.CreateTable<School>();
            }

            if (user is null)
            {
                App.Current.MainPage = new UserSignUpPage();
                return;
            }
            Settings settings = user.GetSettings();
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