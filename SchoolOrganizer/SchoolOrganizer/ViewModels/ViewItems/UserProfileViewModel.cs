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
        public UserProfileViewModel()
        {
            this.TareasCommand = new Command<INavigation>(Tareas);
        }

        private void Tareas(INavigation obj)
        {
            obj.PushModalAsync(new TaskFirstPage(), true);
        }
    }
}
