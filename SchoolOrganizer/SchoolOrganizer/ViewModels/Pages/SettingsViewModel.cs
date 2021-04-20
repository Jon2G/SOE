using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Enums;
using Rg.Plugins.Popup.Services;
using SchoolOrganizer.Views.PopUps;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Kit.Model;
using SchoolOrganizer.Data;
using Xamarin.Forms;
using SchoolOrganizer.Models.Data;

namespace SchoolOrganizer.ViewModels.Pages
{
    public class SettingsViewModel : ModelBase
    {
        public Settings Settings { get; set; }
        public ICommand SaveCommand { get; }
        public ICommand ViewChangeCommand { get; set; }
        public SettingsViewModel()
        {
            this.SaveCommand = new Command(Save);
            ViewChangeCommand = new Command(ViewOpen);
            Settings = AppData.Instance.User.GetSettings();
        }

        private async void ViewOpen(object obj)
        {
            var a = new ViewChangePopUp();
            await a.ShowDialog();
        }

        private void Save()
        {
            Settings.Save();
        }
       
    }

}
