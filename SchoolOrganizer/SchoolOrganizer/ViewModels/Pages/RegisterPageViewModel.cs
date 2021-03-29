using SchoolOrganizer.Models.Scheduler;
using Xamarin.Forms;

namespace SchoolOrganizer.ViewModels.Pages
{
    public class RegisterPageViewModel : BaseViewModel
    {
        public string User { get; set; }
        public string Password { get; set; }
        public Command UserCommand { get; }
        public Command RegisterCommand { get; }

        public RegisterPageViewModel(Subject subject)
        {
            //??
        }
        public RegisterPageViewModel()
        {
            UserCommand = new Command(OnLoginClicked);
           
        }

        private async void OnLoginClicked(object obj)
        {
            Acr.UserDialogs.UserDialogs.Instance.Alert($"Creacion de cuenta exitosa, te damos la bienvenida.", $"!Bienvenido,{User}¡", "Vamos allá");
            //App.Current.MainPage = new AppShell();
       
        }
      
    }
}
