using System.Windows.Input;
using AsyncAwaitBestPractices;
using Kit.Model;
using SOE.Views.PopUps;
using SOE.Views.ViewItems;
using SOE.Views.ViewItems.TasksViews;
using Xamarin.Forms;

namespace SOE.ViewModels.Pages
{
    public class MasterPopUpViewModel : ModelBase
    {
        private readonly MasterPopUp PopUp;
        public bool CanArchieve => MainTaskView.Instance.Model.SelectedIndex == 0;
        public MasterPopUpViewModel(MasterPopUp PopUp)
        {
            this.PopUp = PopUp;
        }
        private ICommand _TapedCommand;
        public ICommand TapedCommand => _TapedCommand ??= new Command<string>(Tapped);

        public string Action { get; private set; }
        private void Tapped(string Action)
        {
            this.Action = Action;
            PopUp.Close().SafeFireAndForget();

        }
    }
}
