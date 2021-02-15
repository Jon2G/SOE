using Xamarin.Forms;

namespace SchoolOrganizer.ViewModels.Pages
{
    public class RegisterPageViewModel : BaseViewModel
    {
        public string Usuario { get; set; }
        public string Password { get; set; }
        public Command UserCommand { get; }
        public Command Registro { get; }

        public RegisterPageViewModel()
        {
            UserCommand = new Command(OnLoginClicked);
           
        }

        private async void OnLoginClicked(object obj)
        {
            Acr.UserDialogs.UserDialogs.Instance.Alert($"Creacion de cuenta exitosa, te damos la bienvenida.", $"!Bienvenido,{Usuario}¡", "Vamos allá");
            App.Current.MainPage = new AppShell();
       
        }
      
    }
}
