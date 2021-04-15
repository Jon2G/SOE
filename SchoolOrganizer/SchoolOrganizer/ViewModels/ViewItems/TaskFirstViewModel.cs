using SchoolOrganizer.Data;
using SchoolOrganizer.Models.Scheduler;
using SchoolOrganizer.Models.TaskFirst;
using SchoolOrganizer.ViewModels.Pages;
using SchoolOrganizer.Views.Pages;
using SchoolOrganizer.Views.PopUps;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace SchoolOrganizer.ViewModels.ViewItems
{
    public class TaskFirstViewModel : BaseViewModel
    {


        public ObservableCollection<ByDayGroup> DayGroups { get; set; }

        public TaskFirstViewModel()
        {

            DayGroups = GetDays();


        }
        private ObservableCollection<ByDayGroup> GetDays()
        {
            ObservableCollection<ByDayGroup> days =
                new(
            AppData.Instance.LiteConnection.DeferredQuery<DateTime>($"SELECT Distinct {nameof(ToDo.Date)} from {nameof(ToDo)} where julianday({nameof(ToDo.Date)})>=date('now')")
                .Select((x) => new ByDayGroup()
                {
                    FDateTime = x
                }).ToList());
            foreach (var day in days)
            {
                day.Refresh(day.FDateTime);
            }
            return days;
        }

    }
}
