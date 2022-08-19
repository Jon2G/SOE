using Kit;
using Kit.Model;
using SOE.Enums;
using SOE.ViewModels.Pages;
using SOE.Views.ViewItems.TasksViews;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SOE.Models.TodoModels
{
    public class ByDayGroup : ModelBase
    {
        public ByDayView View { get; set; }
        public DateTime FDateTime { get; }
        public string Month { get => FDateTime.Month.Mes(); }
        private readonly object _SubjectGroupsLock = new object();

        private ObservableCollection<BySubjectGroup> _SubjectGroups;
        public ObservableCollection<BySubjectGroup> SubjectGroups
        {
            get
            {
                lock (_SubjectGroupsLock)
                {
                    return this._SubjectGroups;
                }
            }
            set
            {
                lock (_SubjectGroupsLock)
                {
                    _SubjectGroups = value;
                    Raise(() => SubjectGroups);
                }
            }
        }
        public int Tareas => SubjectGroups.Sum(x => x.ToDoS.Count);
        private bool _IsExpanded;

        public bool IsExpanded
        {
            get => this._IsExpanded;
            set
            {
                this._IsExpanded = value;
                Raise(() => IsExpanded);
            }
        }
        public ByDayGroup(DateTime date)
        {
            this.FDateTime = date;
            this.SubjectGroups = new ObservableCollection<BySubjectGroup>();
            Binding.EnableCollectionSynchronization(SubjectGroups, _SubjectGroupsLock, CallBack);

        }

        private void CallBack(IEnumerable collection, object context, Action accessmethod, bool writeaccess)
        {
            lock (_SubjectGroupsLock)
            {
                accessmethod.Invoke();
            }
        }

        public async Task ExpandAll(bool expand)
        {
            this.IsExpanded = expand;
            for (int i = 0; i < SubjectGroups.Count; i++)
            {
                SubjectGroups[i].ExpandAll(expand);
            }

            await Task.Delay(500);
            View?.Expander?.ForceUpdateSize();
        }
        internal void RefreshCount()
        {
            Raise(() => Tareas);
        }

        public void Refresh(PendingStatus status)
        {
            //    SubjectGroups.Clear();
            //    SubjectGroups.AddRange(AppData.Instance.LiteConnection.Lista<int>(
            //            $"SELECT Distinct {nameof(ToDo.SubjectId)} from {nameof(ToDo)} where {nameof(ToDo.Date)}={this.FDateTime.Ticks} AND STATUS={(int)status}")
            //        .Select(x => new BySubjectGroup(SubjectService.Get(x))));

            //    foreach (var group in this.SubjectGroups)
            //    {
            //        group.Refresh(this.FDateTime, status);
            //    }
            //    RefreshCount();
        }

        public void Add(ToDo toDo, List<BySubjectGroup> groups)
        {
            try
            {
                BySubjectGroup? group = groups.FirstOrDefault(x => x.Subject == toDo.Subject);
                if (group is not null)
                {
                    group.ToDoS.Add(new TaskViewModel(toDo, group));
                    return;
                }
                BySubjectGroup? newGroup = new BySubjectGroup(toDo.Subject);
                groups.Add(newGroup);
                newGroup.ToDoS.Add(new TaskViewModel(toDo, newGroup));
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Add");
            }
        }
    }
}
