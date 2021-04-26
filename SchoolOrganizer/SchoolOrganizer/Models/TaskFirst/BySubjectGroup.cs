using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Kit.Extensions;
using SchoolOrganizer.Data;
using SchoolOrganizer.Models.Scheduler;
using SchoolOrganizer.Views.Pages;
using SchoolOrganizer.Views.PopUps;
using SchoolOrganizer.Views.ViewItems.TasksViews;
using Xamarin.Forms;

namespace SchoolOrganizer.Models.TaskFirst
{
    public class BySubjectGroup
    {
        private TaskPage taskPage;

        public BySubjectGroupView View { get; set; }
        public Subject Subject { get; set; }
        public ToDo toDo { get; set; }
        public ObservableCollection<ToDo> ToDoS { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand OpenTaskCommand { get; set; }
        public ICommand DetailCommand { get; set; }
        public BySubjectGroup(Subject Subject)
        {
            this.Subject = Subject;
            ToDoS = new ObservableCollection<ToDo>();
            DetailCommand = new Xamarin.Forms.Command<ToDo>(Detail);
            DeleteCommand = new Xamarin.Forms.Command<ToDo>(Eliminar);
            OpenTaskCommand = new Xamarin.Forms.Command<ToDo>(OpenTask);
        }

        public BySubjectGroup(TaskPage taskPage)
        {
            this.taskPage = taskPage;
        }

        public void Eliminar(ToDo todo)
        {
            
            AppData.Instance.LiteConnection.Delete(todo);
            ToDoS.Remove(todo);
            View?.Resize();
        }
        private void OpenTask(ToDo todo)
        {
            this.toDo = todo;
            App.Current.MainPage.Navigation.PushAsync(new TaskPage(), true);
        }
        private void Detail(ToDo obj)
        {
            App.Current.MainPage.Navigation.PushAsync(new TaskDetails(obj), true);
        }

        internal void Refresh(DateTime date)
        {
            this.ToDoS.AddRange(AppData.Instance.LiteConnection.Table<ToDo>()
                .Where(x=>x.SubjectId==this.Subject.Id&&x.Date==date));
            foreach (var todo in ToDoS)
            {
                todo.Subject = Subject;
                todo.LoadDocument();
            }
        }
    }
}
