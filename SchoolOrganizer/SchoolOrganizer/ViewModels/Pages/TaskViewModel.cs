using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Enums;
using Rg.Plugins.Popup.Services;
using SchoolOrganizer.Views.Pages;
using SchoolOrganizer.Views.PopUps;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using FFImageLoading.Forms;
using Xamarin.Forms;
using System.Windows.Input;
using SchoolOrganizer.Models.Scheduler;

namespace SchoolOrganizer.ViewModels.Pages
{

    public class TaskViewModel : BaseViewModel
    {
        public Command TaskCommand { get; }
        private ICommand SubjectSelectedCommand;
        public ICommand SaveCommand { get; }
        private Subject _selectedSubject;

        public ObservableCollection<FileImageSource> Photos { get; }
        public Subject SelectedSubject
        {
            get => _selectedSubject;
            set
            {
                _selectedSubject = value;
                OnPropertyChanged();
            }
        }

        public TaskViewModel()
        {
            TaskCommand = new Command(TaskClicked);
            SubjectSelectedCommand = new Command<SubjectPopUp>(SubjectSelected);
            this.Photos = new ObservableCollection<FileImageSource>();
            this.SaveCommand = new Command(Save);
        }

        private void Save()
        {
          //?
         
        }

        private void SubjectSelected(SubjectPopUp popUp)
        {
            this.SelectedSubject = popUp.Modelo.SelectedSubject;
        }

        private async void TaskClicked(object obj)
        {
            var pr = new SubjectPopUp();
            pr.ClosedCommad = this.SubjectSelectedCommand;
            pr.Mostrar();

            //var scaleAnimation = new ScaleAnimation
            //{
            //    PositionIn = MoveAnimationOptions.Right,
            //    PositionOut = MoveAnimationOptions.Left
            //};

            //pr.Animation = scaleAnimation;
            //await PopupNavigation.Instance.PushAsync(pr);


        }

    }
}
