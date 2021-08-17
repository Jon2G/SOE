using System.Windows.Input;
using AsyncAwaitBestPractices;
using Kit.Model;
using SOE.Enums;
using SOE.Models;
using SOE.Views.PopUps;
using SOE.Views.ViewItems;
using SOE.Views.ViewItems.TasksViews;
using Xamarin.Forms;

namespace SOE.ViewModels.ViewItems
{

    public class MainViewModel : ModelBase
    {
        public ICommand _OpenMenuCommand;
        public ICommand OpenMenuCommand => _OpenMenuCommand ??= new Command(OpenMenu);
        private string _Title;
        public string Title
        {
            get => _Title; set
            {
                _Title = value;
                Raise(() => Title);
            }
        }

        public MainViewModel()
        {
            Title = "Pendientes";
        }
        private async void OpenMenu()
        {

            var pr = new MasterPopUp();
            await pr.ShowDialog();
            Title = pr.Model.Action;
            switch (pr.Model.Action)
            {
                case "Completadas":
                    PendingTasksViewModel.Instance.Refresh(PendingStatus.Done).SafeFireAndForget();
                    PendingRemindersViewModel.Instance.Load(PendingStatus.Done).SafeFireAndForget();
                    break;
                case "Pendientes":
                    PendingTasksViewModel.Instance.Refresh(PendingStatus.Pending).SafeFireAndForget();
                    PendingRemindersViewModel.Instance.Load(PendingStatus.Pending).SafeFireAndForget();
                    break;
                case "Archivadas":
                    PendingTasksViewModel.Instance.Refresh(PendingStatus.Archived).SafeFireAndForget();
                    break;
            }
        }
    }
}
