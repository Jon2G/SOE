using Kit;
using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Enums;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Kit.Model;
using Xamarin.Forms;
using System.Threading.Tasks;
using Acr.UserDialogs;
using System.Collections.Generic;
using AsyncAwaitBestPractices;
using SOE.ViewModels.ViewItems;
using SOE.Views.PopUps;

namespace SOE.ViewModels.Pages
{
    public class MasterPageViewModel : ModelBase
    {
    
        public MasterPageViewModel()
        {

     
        }

        public ICommand _OpenMenuCommand;
        public ICommand OpenMenuCommand => _OpenMenuCommand ??= new Command(OpenMenu);

  
        private async void OpenMenu(object obj)
        {
            var pr = new MasterPopUp();
            await pr.ShowDialog();
            switch (pr.Model.Action)
            {
                case "Completadas":
                    TaskFirstViewModel.Instance.Refresh(TaskFirstViewModel.Done).SafeFireAndForget();
                    break;
                case "Pendientes":
                    TaskFirstViewModel.Instance.Refresh(TaskFirstViewModel.Pending).SafeFireAndForget();
                    break;
                case "Archivadas":
                    TaskFirstViewModel.Instance.Refresh(TaskFirstViewModel.Archived).SafeFireAndForget();
                    break;

            }
        }
 



    }
}