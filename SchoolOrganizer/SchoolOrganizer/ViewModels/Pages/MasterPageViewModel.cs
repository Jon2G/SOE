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
        public ICommand Taskcommand => new Command(TapMenu);



        private void LoadData()
        {
        }
        private void TapMenu()
        {
            var config = new ActionSheetConfig()
            {
                Cancel = new ActionSheetOption("Cancelar"),
                Title = "Opcines de Tareas",
                Message = "Seleccione una opción",
                Options = new List<ActionSheetOption>()
                {
                    new ActionSheetOption("Completadas",null),
                    new ActionSheetOption("Pendientes", null),
                    new ActionSheetOption("Archivadas", null)
                },
                UseBottomSheet = true
                
            };
            Acr.UserDialogs.UserDialogs.Instance.ActionSheet(config);


        }
        private void TaskpageC()
        {
            App.Current.MainPage.Navigation.PushAsync(new TaskPage(), true);
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
