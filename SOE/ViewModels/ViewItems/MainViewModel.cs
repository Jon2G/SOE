using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using AsyncAwaitBestPractices;
using SOE.Enums;
using SOE.Views.Pages;
using SOE.Views.PopUps;
using SOE.Views.ViewItems;
using Xamarin.Forms;

namespace SOE.ViewModels.ViewItems
{

    public class MainViewModel
    {
        public ICommand _OpenMenuCommand;
        public ICommand OpenMenuCommand => _OpenMenuCommand ??= new Command(OpenMenu);

        private ICommand _AddTaskCommand;
        public ICommand AddTaskCommand => _AddTaskCommand ??= new Command(AddTask);
        private async void AddTask()
        {
            var pr = new PrincipalPopUp();
            await pr.ShowDialog();
           
        }
        private async void OpenMenu(object obj)
        {
            var pr = new MasterPopUp();
            await pr.ShowDialog();
            switch (pr.Model.Action)
            {
                case "Completadas":
                    TaskFirstViewModel.Instance.Refresh(ToDoStatus.Done).SafeFireAndForget();
                    break;
                case "Pendientes":
                    TaskFirstViewModel.Instance.Refresh(ToDoStatus.Pending).SafeFireAndForget();
                    break;
                case "Archivadas":
                    TaskFirstViewModel.Instance.Refresh(ToDoStatus.Archived).SafeFireAndForget();
                    break;
            }
        }
    }
}
