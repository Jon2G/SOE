using System;
using System.Windows.Input;
using AsyncAwaitBestPractices;
using SOE.Data;
using SOE.Models.TaskFirst;
using SOE.Views.Pages;
using SOE.Views.PopUps;
using SOE.Views.ViewItems;
using SOE.Views.ViewItems.TasksViews;
using Xamarin.Essentials;
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
                    if (ToDo.Status.HasFlag(Enums.PendingStatus.Done))
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
                    if (ToDo.Status.HasFlag(Enums.PendingStatus.Archived))
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
                    if (DateTime.Now > ToDo.Date)
                    {
                        App.Current.MainPage.DisplayAlert(ToDo.Title,
                            "Esta tarea ya ha expirado, cambie la fecha de entrega si desea compartirla", "Ok.")
                            .SafeFireAndForget();
                        return;
                    }
                    bool IncludeFiles = await App.Current.MainPage.DisplayAlert(ToDo.Title,
                        "¿Compartir también las imágenes de esta tarea?", "Sí", "No");
                    string link = string.Empty;
                    using (Acr.UserDialogs.UserDialogs.Instance.Loading("Compartiendo..."))
                    {
                        link = await Models.TaskFirst.ToDo.Share(ToDo, IncludeFiles);
                    }
                    if (!string.IsNullOrEmpty(link))
                    {
                        Share.RequestAsync(link, "Compartir tarea").SafeFireAndForget();
                    }
                    return;
            }
            PendingTasksView.Instance.Model.Refresh().SafeFireAndForget();
        }
        public void Eliminar()
        {
            AppData.Instance.LiteConnection.Delete(this.ToDo);
            BySubjectGroup.ToDoS.Remove(this);
            BySubjectGroup.View?.Resize();
        }
        private void OpenTask()
        {
            App.Current.MainPage.Navigation.PushAsync(new NewTaskPage(this.ToDo), true);
        }
        private void Detail()
        {
            App.Current.MainPage.Navigation.PushAsync(new TaskDetails(this.ToDo), true);
        }
        private void Archivar()
        {
            this.ToDo.Status |= Enums.PendingStatus.Archived;
            AppData.Instance.LiteConnection.Update(this.ToDo);
        }
        private void Completada()
        {
            this.ToDo.Status = Enums.PendingStatus.Done;
            AppData.Instance.LiteConnection.Update(this.ToDo);
        }
        private void Desarchivar()
        {
            this.ToDo.Status -= Enums.PendingStatus.Archived;
            AppData.Instance.LiteConnection.Update(this.ToDo);
        }
        private void Pendiente()
        {
            this.ToDo.Status -= Enums.PendingStatus.Done;
            this.ToDo.Status |= Enums.PendingStatus.Pending;
            AppData.Instance.LiteConnection.Update(this.ToDo);
        }
    }
}
