using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using SchoolOrganizer.Views.Pages;
using Xamarin.Forms;

namespace SchoolOrganizer.ViewModels.Pages
{
    public class LoginViewModel : BaseViewModel
    {
        public string User { get; set; }
        public string Password { get; set; }
        public Command LoginCommand { get; }
        public Command FingerCommand { get; }
        public Command RegisterCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);
            FingerCommand = new Command(FingerClicked);
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
        private async void FingerClicked(object obj) 
        {
            bool isFingerprintAvailable = await CrossFingerprint.Current.IsAvailableAsync(true);
            if (!isFingerprintAvailable)
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert($"Error",
                    "Biometric authentication is not available or is not configured.", "OK");
                return;
            }

            AuthenticationRequestConfiguration conf =
                new AuthenticationRequestConfiguration("Authentication",
                "Authenticate access to your personal data");

            var authResult = await CrossFingerprint.Current.AuthenticateAsync(conf);
            if (authResult.Authenticated)
            {
                //Success  
                App.Current.MainPage = new AppShell();
            }
            else
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert($"Error", "Authentication failed", "OK");
            }
        }
        private void ConfirmRegister(object obj)
        {
            App.Current.MainPage = new RegisterPage();
            //App.Current.MainPage = new AboutPage();
        }
    }
}
