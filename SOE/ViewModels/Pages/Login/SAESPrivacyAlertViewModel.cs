using System.Linq;
using System.Windows.Input;
using AsyncAwaitBestPractices;
using Kit.Forms.Pages;
using Rg.Plugins.Popup.Services;
using SOE.Views.Pages;
using SOE.Views.PopUps;
using Xamarin.Forms;

namespace SOE.ViewModels.Pages.Login
{
    public class SAESPrivacyAlertViewModel
    {
        public ICommand ContinueCommand { get; set; }
        public ICommand OpenPrivacyCommand { get; set; }
        public SAESPrivacyAlert PopUp { get; set; }
        public SAESPrivacyAlertViewModel(SAESPrivacyAlert saes)
        {
            this.ContinueCommand = new Command(Continue);
            this.OpenPrivacyCommand = new Command(OpenPrivacy);
            this.PopUp = saes;
        }

        private void OpenPrivacy()
        {
            //=> Browser.OpenAsync("www.google.com").SafeFireAndForget();
            App.Current.MainPage.Navigation.PushModalAsync(new PrivacityPage());
            this.Continue();
        }
        private  void Continue() => PopUp?.Close().SafeFireAndForget();
    }
}
