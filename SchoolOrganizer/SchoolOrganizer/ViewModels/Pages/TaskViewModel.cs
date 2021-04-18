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
using Kit.Sql.Base;
using Kit.Sql.Helpers;
using SchoolOrganizer.Data;
using SchoolOrganizer.Models.Scheduler;
using SchoolOrganizer.Models.TaskFirst;

namespace SchoolOrganizer.ViewModels.Pages
{

    public class TaskViewModel : BaseViewModel
    {

        public Command TaskCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand OnDateChangedCommand { get; }
        public ICommand DeleteImageCommand { get; set; }
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

        private ToDo _Tarea;

        public ToDo Tarea
        {
            get => _Tarea;
            set
            {
                _Tarea = value;
                OnPropertyChanged();
            }
        }
        public TaskViewModel()
        {
            Tarea = new ToDo();
            TaskCommand = new Command(TaskClicked);
            SaveCommand = new Command(Save);
            OnDateChangedCommand = new Command(OnDateChanged);
            DeleteImageCommand = new Command<FileImageSource>(DeleteImage);
            this.Photos = new ObservableCollection<FileImageSource>();
        }

        private void OnDateChanged()
        {
            if (this.SelectedSubject != null && this.Tarea?.Date != null)
            {
                this.Tarea.Time =
                    TimeSpan.FromTicks(AppData.Instance.LiteConnection.Single<long>
                        ($"SELECT BEGIN FROM ClassTime WHERE IdSubject={this.SelectedSubject.Id} AND DAY={(int)this.Tarea.Date.DayOfWeek}"));
            }
        }

        private void DeleteImage(FileImageSource img)
        {
            this.Photos.Remove(img);
        }

        private async void Save(object obj)
        {
            this.Tarea.Subject = this.SelectedSubject;
            //insertar o actualizar
            AppData.Instance.LiteConnection.InsertOrReplace(this.Tarea);
            if (Shell.Current is AppShell app)
            {
                await app.MasterPage.TaskFirstPage.Model.Refresh();
            }

            await Shell.Current.Navigation.PopToRootAsync(true);
            //photos ?
        }

        private async void TaskClicked(object obj)
        {
            var pr = new SubjectPopUp();
            await pr.ShowDialog();
            this.SelectedSubject = pr.Modelo.SelectedSubject;
        }

    }
}
