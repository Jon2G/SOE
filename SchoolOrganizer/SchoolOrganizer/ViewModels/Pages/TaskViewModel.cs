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
            this.Photos = new ObservableCollection<FileImageSource>();
            this.SaveCommand = new Command(Save);
        }

        private void Save()
        {
            //?

        }
        private async void TaskClicked(object obj)
        {
            var pr = new SubjectPopUp();
            await pr.ShowDialog();
            this.SelectedSubject = pr.Modelo.SelectedSubject;
        }

    }
}
