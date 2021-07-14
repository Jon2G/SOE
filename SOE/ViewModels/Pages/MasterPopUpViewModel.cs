using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using AsyncAwaitBestPractices;
using Kit.Model;
using SOE.Views.PopUps;
using Xamarin.Forms;

namespace SOE.ViewModels.Pages
{
    public class MasterPopUpViewModel : ModelBase
    {
        private readonly MasterPopUp PopUp;
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
