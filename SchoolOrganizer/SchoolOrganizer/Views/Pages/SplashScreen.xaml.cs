using System;
using System.Security;
using System.Threading.Tasks;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using SchoolOrganizer.Data;
using SchoolOrganizer.Models.Data;
using SchoolOrganizer.ViewModels.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolOrganizer.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SplashScreen : ContentPage
    {

        public SplashScreen()
        {
            InitializeComponent();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await Task.Run(() => { while (Logo.IsLoading) { } });
            AppData.Init();
            User user = User.Get();
            Settings settings = new Settings();
            if (user != null)
            {
                settings = user.GetSettings();
            }

            if (user != null && user.RemeberMe)
            {
                if (settings.IsFingerPrintActive)
                {
                    if (!await CrossFingerprint.Current.IsAvailableAsync(true))
                    {
                        Acr.UserDialogs.UserDialogs.Instance.Alert("La autenticación biométrica no esta disponible  o no esta configurada.", "Atención", "OK");
                        GotoManualLogin(user);
                    }
                    var authResult = await CrossFingerprint.Current.AuthenticateAsync(
                        new AuthenticationRequestConfiguration("Bloqueo de aplicación",
                        "Inicio de sesíon por huella")
                        {
                            AllowAlternativeAuthentication = true
                        });
                    if (authResult.Authenticated)
                    {
                        GotoApp(user, settings);
                    }
                    else
                    {
                        GotoManualLogin(user);
                    }
                }

            }
            else
            {
                App.Current.MainPage = new LoginPage();
            }


            //Si la llamada se provoco desde el WidgetDeHorario
            //if (Widgets.Horario.WidgetHorario.Instance.IsItentDataSet())
            //{
            //    Widgets.Horario.IntentData data = Widgets.Horario.WidgetHorario.Instance.GetIntentData();
            //    UserDialogs.Instance.Alert($"Se presiono el elemento #{data.RowId+1}", data.NombreMateria, "Ok");
            //}

            ////Si la llamada se provoco desde el Widget Tareas
            //if (Widgets.Tareas.WidgetTareas.Instance.IsItentDataSet())
            //{
            //    Widgets.Tareas.IntentData data = Widgets.Tareas.WidgetTareas.Instance.GetIntentData();
            //    UserDialogs.Instance.Alert($"Se presiono el elemento #{data.RowId + 1}", data.NombreTarea, "Ok");
            //}


            //App.Current.MainPage = new LoginPage();
        }

        private void GotoManualLogin(User user)
        {
            Acr.UserDialogs.UserDialogs.Instance.Alert("TODO", "No hay no existe");
        }

        private void GotoApp(User user, Settings settings)
        {
            AppData.Instance.User = user;
            App.Current.MainPage = new AppShell();
            settings.Notifications();
        }
    }
}