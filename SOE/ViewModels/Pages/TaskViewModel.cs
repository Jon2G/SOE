using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using SOE.Data;
using SOE.Models.TaskFirst;
using SOE.Views.Pages;
using SOE.Views.PopUps;
using Xamarin.Forms;

namespace SOE.ViewModels.Pages
{
    public class TaskViewModel
    {
        public ICommand _OpenMenuCommand;
        public ICommand OpenMenuCommand => _OpenMenuCommand ??= new Command(OpenMenu);
        public ToDo ToDo { get; set; }
        private readonly BySubjectGroup BySubjectGroup;
        public TaskViewModel(ToDo ToDo, BySubjectGroup BySubjectGroup)
        {
            this.BySubjectGroup = BySubjectGroup;
            this.ToDo = ToDo;
        }
        private async void OpenMenu(object obj)
        {
            var pr = new MenuPopUp(ToDo);
            await pr.ShowDialog();
            switch (pr.Model.Action)
            {
                case "Ver":
                    Detail();
                    break;
                case "Hecho":
                    if (ToDo.Done)
                    {
                        Pendiente();
                        break;
                    }
                    Completada();
                    break;
                case "Editar":
                    OpenTask();
                    break;
                case "Archivar":
                    if (ToDo.Archived)
                    {
                        Desarchivar();
                        break;
                    }
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
            this.ToDo.Archived = true;
            AppData.Instance.LiteConnection.Update(this.ToDo);
        }
        private void Completada()
        {
            this.ToDo.Done = true;
            AppData.Instance.LiteConnection.Update(this.ToDo);
        }
        private void Desarchivar()
        {
            this.ToDo.Archived = false;
            AppData.Instance.LiteConnection.Update(this.ToDo);
        }
        private void Pendiente()
        {
            this.ToDo.Done = false;
            AppData.Instance.LiteConnection.Update(this.ToDo);
        }
    }
}
