using SchoolOrganizer.Views.Pages;
using Xamarin.Forms;

namespace SchoolOrganizer.ViewModels.Pages
{
    public class LoginViewModel : BaseViewModel
    {
        public string User { get; set; }
        public string Password { get; set; }
        public Command LoginCommand { get; }
        public Command RegisterCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);
            RegisterCommand = new Command(ConfirmRegister);
        }

        private void OnLoginClicked(object obj)
        {
            Acr.UserDialogs.UserDialogs.Instance.Alert($"Hemos notado que es tu primer inicio de sesión, te damos la bienvenida.", $"!Bienvenido,{User}¡", "Vamos allá");
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            /*await Shell.Current.GoToAsync($"//{nameof(AboutPage)}", true);*/
            //await App.Current.MainPage.Navigation.PushAsync(new LoginPage());
            App.Current.MainPage = new AppShell();
        }
        private void ConfirmRegister(object obj)
        {
            App.Current.MainPage = new RegisterPage();
            //App.Current.MainPage = new AboutPage();
        }
    }
}
