using System.Windows.Input;
using Kit.Forms.Model;
using Kit.Model;
using SOE.Data;
using SOE.Models.Data;
using SOE.Views.PopUps;
using Xamarin.Forms;

namespace SOE.ViewModels.Pages.Login
{

    class LoginPopUpViewModel :ValidationsModelbase
    {

        User user = User.Get();
        private string _Boleta;
        private string _Password;
        [Validations.Boleta(ErrorMessage = "Boleta no valida.")]
        public string Boleta { get => _Boleta; set { _Boleta = value; ValidateProperty(value); Raise(() => Boleta); } }
        public string Password { get => _Password; set { _Password = value; Raise(() => Password); } }
        public ICommand IngresarCommand { get; set; }
        public LoginPopUpViewModel()
        {
            this.Boleta = user.Boleta;
            IngresarCommand = new Command<LoginPopUp>(Ingresar);
        }

        private async void Ingresar(LoginPopUp obj)
        {
            if (this.Password == user.Password)
            {
                AppData.Instance.User = user;
                await obj.Close();
            }
            else
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert("Usuario  o contraseña incorrectos.", "Error");
            }
        }
    }
}
