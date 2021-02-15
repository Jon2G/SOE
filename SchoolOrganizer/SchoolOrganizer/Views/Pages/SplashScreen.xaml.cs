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
        protected override void OnAppearing()
        {
            base.OnAppearing();

            
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


            App.Current.MainPage = new LoginPage();
        }
    }
}