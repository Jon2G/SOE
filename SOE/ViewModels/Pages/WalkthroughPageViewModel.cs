using System.Windows.Input;
using Kit.Extensions;
using Kit.Model;

namespace SOE.ViewModels.Pages
{
    public class WalkthroughPageViewModel : ModelBase
    {
        private ICommand _ContinueCommand;
        public ICommand ContinueCommand => _ContinueCommand ??= new Command(Continue);

        private void Continue() => App.Current.MainPage = new AppShell();
    }
}
