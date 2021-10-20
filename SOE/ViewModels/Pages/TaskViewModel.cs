using System;
using System.Windows.Input;
using AsyncAwaitBestPractices;
using SOE.Data;
using SOE.Models.TodoModels;
using SOE.Models.TodoModels;
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
                    return;
                case "Hecho":
                    if (ToDo.Status == Enums.PendingStatus.Done)
                    {
                        Pendiente();
                    }
                    else
                    {
                        Completada();
                    }
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
                    if (DateTime.Now > ToDo.Date.Add(ToDo.Time))
                    {
                        App.Current.MainPage.DisplayAlert(ToDo.Title,
                            "Esta tarea ya ha expirado, cambie la fecha de entrega si desea compartirla", "Ok.")
                            .SafeFireAndForget();
                        return;
                    }
                    string link = string.Empty;
                    using (Acr.UserDialogs.UserDialogs.Instance.Loading("Compartiendo..."))
                    {
                        link = await ToDo.Share(ToDo);
                    }
                    if (!string.IsNullOrEmpty(link))
                    {
                        Share.RequestAsync(link, "Compartir tarea").SafeFireAndForget();
                    }
                    return;
            }
            Device.BeginInvokeOnMainThread(() =>
                PendingTasksView.Instance?.Model.Refresh(PendingTasksView.Instance?.OnRefreshCompleteAction,ToDo.Status));
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
            MainView.Instance.Model.Title = "Archivadas";
            this.ToDo.Status |= Enums.PendingStatus.Archived;
            AppData.Instance.LiteConnection.Update(this.ToDo);
        }
        private void Completada()
        {
            MainView.Instance.Model.Title = "Completadas";
            this.ToDo.Status = Enums.PendingStatus.Done;
            AppData.Instance.LiteConnection.Update(this.ToDo);
        }
        private void Desarchivar()
        {
            MainView.Instance.Model.Title = "Pendientes";
            this.ToDo.Status -= Enums.PendingStatus.Archived;
            AppData.Instance.LiteConnection.Update(this.ToDo);
        }
        private void Pendiente()
        {
            MainView.Instance.Model.Title = "Pendientes";
            this.ToDo.Status -= Enums.PendingStatus.Done;
            this.ToDo.Status |= Enums.PendingStatus.Pending;
            AppData.Instance.LiteConnection.Update(this.ToDo);
        }
    }
}
