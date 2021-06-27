using System;
using System.Collections.ObjectModel;
using System.Linq;
using APIModels;
using Kit;
using Kit.Model;
using SOE.Data;
using SOE.Models.Academic;
using SOE.Models.Scheduler;
using SOE.Services;
using SOE.Views.ViewItems.TasksViews;

namespace SOE.Models.TaskFirst
{
    public class ByDayGroup : ModelBase
    {
        public ByDayView View { get; set; }
        public DateTime FDateTime { get; set; }
        public string Month { get => FDateTime.Month.Mes(); }
        public ObservableCollection<BySubjectGroup> SubjectGroups { get; set; }
        public int Tareas => SubjectGroups.Sum(x => x.ToDoS.Count);

        public ByDayGroup()
        {
            this.SubjectGroups = new ObservableCollection<BySubjectGroup>();

        }

        internal void RefreshCount()
        {
            Raise(() => Tareas);
        }

        internal void Refresh()
        {
            SubjectGroups.Clear();
            SubjectGroups.AddRange(AppData.Instance.LiteConnection.Lista<int>(
                    $"SELECT Distinct {nameof(ToDo.SubjectId)} from {nameof(ToDo)}")
                .Select(x => new BySubjectGroup(SubjectService.Get(x))));
            //??????????????????????? le movi aqui where {nameof(ToDo.Date)}<{this.FDateTime.Ticks}

            foreach (var group in this.SubjectGroups)
            {
                group.Refresh(this.FDateTime);
            }
            RefreshCount();
        }
    }
}
