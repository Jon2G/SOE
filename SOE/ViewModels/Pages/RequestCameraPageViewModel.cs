using AsyncAwaitBestPractices.MVVM;
using Kit.Forms.Extensions;
using Kit.Forms.Pages;
using Rg.Plugins.Popup.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;

namespace SOE.ViewModels.Pages
{
    public class RequestCameraPageViewModel
    {
        public ICommand ContinueCommand { get; set; }
        public RequestCameraPageViewModel()
        {
            this.ContinueCommand = new AsyncCommand(Continue);
        }

        public static bool ShouldAsk()
        {
            if (Permisos.IsDisabled(new Permissions.Camera()))
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert("Ha denegado el acceso a su camera, por favor permita el acceso desde ajustes de su dispositivo", "Alerta", "Ok");
                return false;
            }
            if (Permisos.IsDisabled(new Permissions.Photos()))
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert("Ha denegado el acceso a la galeria,por favor permita el acceso desde ajustes de su dispositivo", "Alerta", "Ok");
                return false;
            }

            return true;
        }
        private async Task Continue()
        {
            await Permisos.PedirPermiso(new Permissions.Camera(), "Por favor permita el acceso");
            await Permisos.PedirPermiso(new Permissions.Photos(), "Por favor permita el acceso");
            await (PopupNavigation.Instance.PopupStack.First() as BasePopUp)?.Close();
        }
    }
}
