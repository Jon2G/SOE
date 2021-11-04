using System.Linq;
using System.Windows.Input;
using Kit.Forms.Extensions;
using Kit.Forms.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SOE.ViewModels.Pages
{
    public class RequestCameraPageViewModel
    {
        public ICommand ContinueCommand { get; set; }
        public RequestCameraPageViewModel()
        {
            this.ContinueCommand = new Command(Continue);
        }

        public static bool ShouldAsk()
        {
            if (Permisos.IsDisabled(new Permissions.Camera()))
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert("Ha denegado el acceso a su camera, por favor permita el acceso desde ajustes de su dispositivo","Alerta", "Ok");
                return false;
            }
            if (Permisos.IsDisabled(new Permissions.Photos()))
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert("Ha denegado el acceso a la galeria,por favor permita el acceso desde ajustes de su dispositivo","Alerta", "Ok");
                return false;
            }

            return true;
        }
        private async void Continue()
        {
            await Permisos.PedirPermiso(new Permissions.Camera(), "Por favor permita el acceso");
            await Permisos.PedirPermiso(new Permissions.Photos(), "Por favor permita el acceso");
            await (PopupNavigation.Instance.PopupStack.First() as BasePopUp)?.Close();
        }
    }
}
