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
            this.SettingCommand = new Command(OpenSettings);
            this.TareasCommand = new Command(Tareas);
        }

        private void Tareas()
        {
            //Shell.Current.Navigation.PushAsync(new TaskFirstPage(), true);
        }

        private void OpenSettings()
        {
            Shell.Current.Navigation.PushModalAsync(new SettingsView(), true);
        }
    }
}
