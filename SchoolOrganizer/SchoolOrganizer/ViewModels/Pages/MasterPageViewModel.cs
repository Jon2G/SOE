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

namespace SchoolOrganizer.ViewModels.Pages
{
    class MasterPageViewModel : ModelBase
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

        public ICommand ItemSelectedCommand => new Command<string>(ItemSelected);

        private void LoadData()
        {
            //var tasks = TaskService.Instance.GetTasks();

            //Tasks.Clear();
            //foreach (var task in tasks)
            //{
            //    Tasks.Add(task);
            //}
        }

        private void ItemSelected(string parameter)
        {
            // App.Current.MainPage = new TaskPage();
            switch (parameter)
            {
                case "Delayed":
                    //App.Current.MainPage = new TaskPage();
                    App.Current.MainPage.Navigation.PushAsync(new TaskPage(), true);
                    //var pr = new TaskPage();
                    //var scaleAnimation = new ScaleAnimation
                    //{
                    //    PositionIn = MoveAnimationOptions.Right,
                    //    PositionOut = MoveAnimationOptions.Left
                    //};

                    //pr.Animation = scaleAnimation;
                    //await PopupNavigation.Instance.PushAsync(pr);
                    break;
            }

        }
    }
}
