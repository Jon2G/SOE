using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using APIModels;
using AsyncAwaitBestPractices;
using Kit;
using Kit.Extensions;
using Rg.Plugins.Popup.Services;
using SOE.Data;
using SOE.Enums;
using SOE.ViewModels.Pages;
using SOE.Views.ViewItems.TasksViews;
using Xamarin.Forms;

namespace SOE.Models.TaskFirst
{
    public class BySubjectGroup
    {
        public BySubjectGroupView View { get; set; }
        public Subject Subject { get; set; }

        public ObservableCollection<TaskViewModel> ToDoS { get; set; }

        public BySubjectGroup(Subject Subject)
        {
            this.Subject = Subject;
            ToDoS = new ObservableCollection<TaskViewModel>();
        }

        internal void Refresh(DateTime date, ToDoStatus status)
        {
            this.ToDoS.AddRange(AppData.Instance.LiteConnection
                .DeferredQuery<ToDo>($"SELECT * from {nameof(ToDo)} where {nameof(ToDo.SubjectId)}=? AND STATUS=? AND {nameof(ToDo.Date)}=? order by Time", this.Subject.Id,status, date)
                .Select(x => new TaskViewModel(x, this)));
            foreach (var todo in ToDoS)
            {
                todo.ToDo.Subject = Subject;
                todo.ToDo.LoadDocument();
            }
        }
    }
}