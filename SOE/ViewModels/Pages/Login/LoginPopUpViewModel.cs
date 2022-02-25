using AsyncAwaitBestPractices.MVVM;
using System.Windows.Input;
using Kit.Forms.Model;
using Kit.Model;
using SOE.Data;
using SOE.Models.Data;
using SOE.Validations;
using SOE.Views.PopUps;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace SOE.ViewModels.Pages.Login
{
    [Preserve(AllMembers = true)]
    class LoginPopUpViewModel : ValidationsModelbase
    {
        private string _Boleta;
        private string _Password;
        [BoletaValidation(ErrorMessage = "Boleta no valida.")]
        public string Boleta { get => _Boleta; set { _Boleta = value; ValidateProperty(value); Raise(() => Boleta); } }
        public string Password { get => _Password; set { _Password = value; Raise(() => Password); } }
        public ICommand IngresarCommand { get; set; }
        public LoginPopUpViewModel()
        {
            this.Boleta = UserLocalData.Instance.Boleta;
            IngresarCommand = new AsyncCommand<LoginPopUp>(Ingresar);
        }

        private async Task Ingresar(LoginPopUp obj)
        {
            if (this.Password == UserLocalData.Instance.Password)
            {
                AppData.Instance.User = await User.Get();
                await obj.Close();
            }
            else
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert("Usuario  o contraseña incorrectos.", "Error");
            }
        }
    }
}
