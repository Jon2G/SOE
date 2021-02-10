using OrganizadorEscolar.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace OrganizadorEscolar.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public string Usuario { get; set; }
        public string Password { get; set; }
        public Command LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);
        }

        private async void OnLoginClicked(object obj)
        {
            Acr.UserDialogs.UserDialogs.Instance.Alert($"Hemos notado que es tu primer inicio de sesión, te damos la bienvenida.", $"!Bienvenido,{Usuario}¡", "Vamos allá");
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            await Shell.Current.GoToAsync($"//{nameof(AboutPage)}", true);
        }
    }
}
