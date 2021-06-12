using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Kit;
using Kit.Extensions;
using Kit.Model;
using SchoolOrganizer.Data;
using SchoolOrganizer.Models.Scheduler;
using SchoolOrganizer.Views.ViewItems.TasksViews;
using Xamarin.Forms;

namespace SchoolOrganizer.Models.TaskFirst
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
                    $"SELECT Distinct {nameof(ToDo.SubjectId)} from {nameof(ToDo)} where {nameof(ToDo.Date)}={this.FDateTime.Ticks}")
                .Select(x => new BySubjectGroup(Subject.Get(x))));
            //???????????????????????
            
            foreach (var group in this.SubjectGroups)
            {
                group.Refresh(this.FDateTime);
            }
            RefreshCount();
        }
    }
}
