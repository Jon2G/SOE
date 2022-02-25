using Kit.Model;
using SOE.Enums;
using SOE.Views.PopUps;
using SOE.Views.ViewItems;
using System.Windows.Input;
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
                    PendingTasksView.Instance?.Model.Refresh(PendingTasksView.Instance?.OnRefreshCompleteAction, PendingStatus.Done);
                    PendingRemindersViewModel.Instance.Load(PendingStatus.Done);
                    break;
                case "Pendientes":
                    PendingTasksView.Instance?.Model.Refresh(PendingTasksView.Instance?.OnRefreshCompleteAction);
                    PendingRemindersViewModel.Instance.Load(PendingStatus.Pending);
                    break;
                case "Archivadas":
                    PendingTasksView.Instance?.Model.Refresh(PendingTasksView.Instance?.OnRefreshCompleteAction, PendingStatus.Archived);
                    PendingRemindersViewModel.Instance.Load(PendingStatus.Pending);
                    break;
            }
        }
    }
}
