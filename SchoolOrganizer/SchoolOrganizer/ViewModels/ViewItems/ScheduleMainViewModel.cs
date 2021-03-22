using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using Kit.Model;
using SchoolOrganizer.Models.Scheduler;
using SchoolOrganizer.ViewModels.ViewItems.ScheduleViewModel;

namespace SchoolOrganizer.ViewModels.ViewItems
{
    public class ScheduleMainViewModel : ModelBase
    {
        public ObservableCollection<SheduleDay> WeekDays { get; set; }
        public ObservableCollection<Hour> WeekHours { get; set; }

        public ScheduleMainViewModel()
        {
            WeekDays = new ObservableCollection<SheduleDay>();
            WeekHours = new ObservableCollection<Hour>();
            GetWeek();

        }

        public void GetWeek()
        {
            int min_hour = 25;
            int max_hour = -1;
            for (DayOfWeek dayOfWeek = DayOfWeek.Monday; dayOfWeek <= DayOfWeek.Friday; dayOfWeek++)
            {
                var schedule = new SheduleDay(Day.GetNearest(dayOfWeek));
                if (!schedule.Class.Any()) { continue; }
                WeekDays.Add(schedule);
                int max_newhour = schedule.Class.Max(x => x.End.Hours);
                int min_newhour = schedule.Class.Min(x => x.Begin.Hours);
                if (max_newhour > max_hour)
                {
                    max_hour = max_newhour;
                }
                if (min_newhour < min_hour)
                {
                    min_hour = min_newhour;
                }
            }

            int j = 0;
            for (int i = min_hour; i < max_hour; i++, j++)
            {
                this.WeekHours.Add(new Hour(j, i));
            }
        }
    }
}
