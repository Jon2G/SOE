using System.Linq;
using System.Windows.Input;
using AsyncAwaitBestPractices;
using Kit.Forms.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SOE.ViewModels.Pages.Login
{
    public class SAESPrivacyAlertViewModel
    {
        public ICommand ContinueCommand { get; set; }
        public ICommand OpenPrivacyCommand { get; set; }
        public SAESPrivacyAlertViewModel()
        {
            this.ContinueCommand = new Command(Continue);
            this.OpenPrivacyCommand = new Command(OpenPrivacy);
        }

        private  void OpenPrivacy() => Browser.OpenAsync("www.google.com").SafeFireAndForget();
        private  void Continue() => (PopupNavigation.Instance.PopupStack.First() as BasePopUp)?.Close().SafeFireAndForget();
    }
}
