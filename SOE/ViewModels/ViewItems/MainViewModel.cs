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


        private async void OpenMenu(object obj)
        {
            var pr = new MasterPopUp();
            await pr.ShowDialog();
            switch (pr.Model.Action)
            {
                case "Completadas":
                    PendingTasksViewModel.Instance.Refresh(ToDoStatus.Done).SafeFireAndForget();
                    break;
                case "Pendientes":
                    PendingTasksViewModel.Instance.Refresh(ToDoStatus.Pending).SafeFireAndForget();
                    break;
                case "Archivadas":
                    PendingTasksViewModel.Instance.Refresh(ToDoStatus.Archived).SafeFireAndForget();
                    break;
            }
        }
    }
}
