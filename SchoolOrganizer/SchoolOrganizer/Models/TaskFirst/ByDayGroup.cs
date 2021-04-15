using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
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
        public string Month { get => Kit.Extensions.Helpers.Mes(FDateTime.Month); }
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

        internal void Refresh(DateTime date)
        {
            SubjectGroups.Clear();
            var subjects_ids = AppData.Instance.LiteConnection.DeferredQuery<int>($"SELECT Distinct {nameof(ToDo.SubjectId)} from {nameof(ToDo)} where julianday({nameof(ToDo.Date)})=?", this.FDateTime).ToList();
            SubjectGroups.AddRange(subjects_ids.Select(x => new BySubjectGroup(Subject.Get(x))).ToList());
            foreach(var group in this.SubjectGroups)
            {
                group.Refresh(date);
            }
            RefreshCount();
        }
    }
}
