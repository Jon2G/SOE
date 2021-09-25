using System;
using System.Collections.ObjectModel;
using System.Linq;
using Kit;
using Kit.Model;
using SOE.Data;
using SOE.Enums;
using SOE.Services;
using SOE.Views.ViewItems.TasksViews;
using System.Threading.Tasks;

namespace SOE.Models.TaskFirst
{
    public class ByDayGroup : ModelBase
    {
        public ByDayView View { get; set; }
        public DateTime FDateTime { get; }
        public string Month { get => FDateTime.Month.Mes(); }
        public ObservableCollection<BySubjectGroup> SubjectGroups { get; set; }
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
            SubjectGroups.Clear();
            SubjectGroups.AddRange(AppData.Instance.LiteConnection.Lista<int>(
                    $"SELECT Distinct {nameof(ToDo.SubjectId)} from {nameof(ToDo)} where {nameof(ToDo.Date)}={this.FDateTime.Ticks} AND STATUS={(int)status}")
                .Select(x => new BySubjectGroup(SubjectService.Get(x))));
            //??????????????????????? le movi aqui where {nameof(ToDo.Date)}<={this.FDateTime.Ticks}

            foreach (var group in this.SubjectGroups)
            {
                group.Refresh(this.FDateTime, status);
            }
            RefreshCount();
        }
    }
}
