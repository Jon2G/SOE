using AsyncAwaitBestPractices;
using Kit.Forms.Pages;
using Kit.Model;
using Rg.Plugins.Popup.Services;
using SchoolOrganizer.Views.PopUps;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace SchoolOrganizer.ViewModels.Pages
{
   public class MenuPopUpViewModel: ModelBase
    {
        private readonly MenuPopUp PopUp;
        public MenuPopUpViewModel(MenuPopUp PopUp)
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
