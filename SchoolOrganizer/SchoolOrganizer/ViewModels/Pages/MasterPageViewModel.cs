using Kit;
using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Enums;
using Rg.Plugins.Popup.Services;
using SchoolOrganizer.Views.PopUps;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Kit.Model;
using Xamarin.Forms;
using System.Threading.Tasks;
using Acr.UserDialogs;
using System.Collections.Generic;
using SchoolOrganizer.ViewModels.ViewItems;
using AsyncAwaitBestPractices;

namespace SchoolOrganizer.ViewModels.Pages
{
    public class MasterPageViewModel : ModelBase
    {
        //private ObservableCollection<Models.Task> _tasks;

        public MasterPageViewModel()
        {
            // Tasks = new ObservableCollection<Models.Task>();

            LoadData();
        }

        //public ObservableCollection<Models.Task> Tasks
        //{
        //    get { return _tasks; }
        //    set
        //    {
        //        _tasks = value;
        //        OnPropertyChanged();
        //    }
        //}

        public ICommand ItemSelectedCommand => new Command(TaskpageC);
        public ICommand _OpenMenuCommand;
        public ICommand OpenMenuCommand => _OpenMenuCommand ??= new Command(OpenMenu);

        private void LoadData()
        {
        }
        private async void OpenMenu(object obj)
        {
            var pr = new MasterPopUp();
            await pr.ShowDialog();
            switch (pr.Model.Action)
            {
                case "Completadas":
                    TaskFirstViewModel.Instance.Refresh(TaskFirstViewModel.Done ).SafeFireAndForget();
                    break;
                case "Pendientes":
                    TaskFirstViewModel.Instance.Refresh(TaskFirstViewModel.Pending ).SafeFireAndForget();
                    break;
                case "Archivadas":
                    TaskFirstViewModel.Instance.Refresh(TaskFirstViewModel.Archived ).SafeFireAndForget();
                    break;

            }
        }
        private void TaskpageC()
        {
            App.Current.MainPage.Navigation.PushAsync(new TaskPage(), true);
        }
        //    private void ItemSelected(string parameter)
        //{
        //    // App.Current.MainPage = new TaskPage();
        //    switch (parameter)
        //    {
        //        case "Delayed":
        //            //App.Current.MainPage = new TaskPage();
        //            App.Current.MainPage.Navigation.PushAsync(new TaskPage(), true);
        //            //var pr = new TaskPage();
        //            //var scaleAnimation = new ScaleAnimation
        //            //{
        //            //    PositionIn = MoveAnimationOptions.Right,
        //            //    PositionOut = MoveAnimationOptions.Left
        //            //};

        //            //pr.Animation = scaleAnimation;
        //            //await PopupNavigation.Instance.PushAsync(pr);
        //            break;
        //    }

        
    }
}
