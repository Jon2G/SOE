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
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            AppData.Init();
            User user = AppData.Instance.LiteConnection.Table<User>().FirstOrDefault();
            if (user != null && user.RemeberMe)
            {
                bool isFingerprintAvailable = await CrossFingerprint.Current.IsAvailableAsync(true);
                if (!isFingerprintAvailable)
                {
                    Acr.UserDialogs.UserDialogs.Instance.Alert($"Error",
                        "La autenticacion biometrica no esta disponible  o no esta configurada.", "OK");
                    return;
                }

                AuthenticationRequestConfiguration conf =
                    new AuthenticationRequestConfiguration("Authentication",
                    "Authenticate access to your personal data");
                conf.AllowAlternativeAuthentication = true;
                var authResult = await CrossFingerprint.Current.AuthenticateAsync(conf);
                if (authResult.Authenticated)
                {
                    AppData.Instance.User = user;
                    App.Current.MainPage = new AppShell();
                }
                else
                {
                    Acr.UserDialogs.UserDialogs.Instance.Alert($"Error", "Autenticacion fallida", "OK");
                }
                AppData.Instance.User = user;
                       App.Current.MainPage = new AppShell();
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
    }
}