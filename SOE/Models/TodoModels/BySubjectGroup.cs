using System;
using System.Collections.ObjectModel;
using System.Linq;
using SOEWeb.Shared;
using Kit;
using Kit.Model;
using SOE.Data;
using SOE.Enums;
using SOE.ViewModels.Pages;
using SOE.Views.ViewItems.TasksViews;
using SOE.Models.TodoModels;

namespace SOE.Models.TodoModels
{
    public class BySubjectGroup : ModelBase
    {
        public BySubjectGroupView View { get; set; }
        public Subject Subject { get; set; }
        private bool _IsExpanded;

        public bool IsExpanded
        {
            get => this._IsExpanded;
            set
            {
                this._IsExpanded = value;
                if (value)
                {
                    View?.Expander?.ForceUpdateSize();
                }
                Raise(() => IsExpanded);
            }
        }

        public ObservableCollection<TaskViewModel> ToDoS { get; set; }

        public BySubjectGroup(Subject Subject)
        {
            this.Subject = Subject;
            ToDoS = new ObservableCollection<TaskViewModel>();
        }

        public void ExpandAll(bool expand)
        {
            this.IsExpanded = expand;
        }

        internal void Refresh(DateTime date, PendingStatus status)
        {
            this.ToDoS.AddRange(AppData.Instance.LiteConnection
                .DeferredQuery<ToDo>(
                    $"SELECT * from {nameof(ToDo)} where {nameof(ToDo.SubjectId)}=? AND STATUS=? AND {nameof(ToDo.Date)}=? order by Time",
                    this.Subject.Id, status, date)
                .Select(x => new TaskViewModel(x, this)));
            foreach (var todo in ToDoS)
            {
                todo.ToDo.Subject = Subject;
                todo.ToDo.LoadDocument();
            }
        }


    }
}