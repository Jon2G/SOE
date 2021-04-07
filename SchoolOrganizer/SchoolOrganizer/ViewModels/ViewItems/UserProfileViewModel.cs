using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Kit;
using Kit.Model;
using SchoolOrganizer.Views.Pages;
using Xamarin.Forms;

namespace SchoolOrganizer.ViewModels.ViewItems
{
    public class UserProfileViewModel : ModelBase
    {
        public ICommand TareasCommand { get; }
        public ICommand SettingCommand { get; set; }

        public UserProfileViewModel()
        {
           this.SettingCommand = new Command<INavigation>(OpenSettings);
            this.TareasCommand = new Command<INavigation>(Tareas);
        }

        private void Tareas(INavigation obj)
        {
            obj.PushModalAsync(new TaskFirstPage(), true);
        }

        private void OpenSettings(INavigation obj)
        {
            obj.PushModalAsync(new SettingsView(),true);
        }
    }
}
