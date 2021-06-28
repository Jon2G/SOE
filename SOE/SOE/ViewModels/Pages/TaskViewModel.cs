using AsyncAwaitBestPractices;
using SchoolOrganizer.Data;
using SchoolOrganizer.Models.TaskFirst;
using SchoolOrganizer.Views.Pages;
using SchoolOrganizer.Views.PopUps;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace SchoolOrganizer.ViewModels.Pages
{
    public class TaskViewModel
    {
        public ICommand _OpenMenuCommand;
        public ICommand OpenMenuCommand => _OpenMenuCommand ??= new Command(OpenMenu);
        public ToDo ToDo { get; set; }
        private readonly BySubjectGroup BySubjectGroup;
        public TaskViewModel(ToDo ToDo,BySubjectGroup BySubjectGroup)
        {
            this.BySubjectGroup = BySubjectGroup;
            this.ToDo = ToDo;
        }
        private async void OpenMenu(object obj)
        {
            var pr = new MenuPopUp();
            await pr.ShowDialog();
            switch(pr.Model.Action)
            {
                case "Ver":
                    Detail();
                    break;
                case "Hecho":
                    Completada();
                    break;
                case "Editar":
                    OpenTask();
                    break;
                case "Archivar":
                    Archivar();
                    break;
                case "Eliminar":
                    Eliminar();
                    break;
                case "Compartir":
                    break;
            }
        }
        public void Eliminar()
        {
            AppData.Instance.LiteConnection.Delete(this.ToDo);
            BySubjectGroup.ToDoS.Remove(this);
            BySubjectGroup.View?.Resize();
        }
        private void OpenTask()
        {
            App.Current.MainPage.Navigation.PushAsync(new TaskPage(this.ToDo), true);
        }
        private void Detail()
        {
            App.Current.MainPage.Navigation.PushAsync(new TaskDetails(this.ToDo), true);
        }
        private void Archivar()
        {
            this.ToDo.Archived = 1;
            AppData.Instance.LiteConnection.Update(this.ToDo);
        }
        private void Completada()
        {
            this.ToDo.Done = 1;
            AppData.Instance.LiteConnection.Update(this.ToDo);
        }

    }
}
