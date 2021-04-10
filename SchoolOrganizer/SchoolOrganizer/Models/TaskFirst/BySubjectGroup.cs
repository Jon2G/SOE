using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using SchoolOrganizer.Models.Scheduler;
using SchoolOrganizer.Views.ViewItems.TasksViews;
using Xamarin.Forms;

namespace SchoolOrganizer.Models.TaskFirst
{
    public class BySubjectGroup
    {
        public BySubjectGroupView View { get; set; }
        public Subject Subject { get; set; }
        public ObservableCollection<ToDo> ToDoS { get; set; }
        public ICommand DeleteCommand { get; set; }
        public BySubjectGroup(Subject Subject)
        {
            this.Subject = Subject;
            ToDoS = new ObservableCollection<ToDo>();
            DeleteCommand = new Command<ToDo>(Eliminar);
        }
        public void Eliminar(ToDo todo)
        {
            ToDoS.Remove(todo);
            View?.Resize();
        }
    }
}
