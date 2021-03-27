using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Windows.Input;
using Kit.Model;
using SchoolOrganizer.Models.Scheduler;
using SchoolOrganizer.ViewModels.ViewItems.ScheduleViewModel;
using Xamarin.Forms;
using System.Runtime.CompilerServices;
using System.Threading;
using Kit.Forms.Extensions;

namespace SchoolOrganizer.ViewModels.ViewItems
{
    public class ScheduleMainViewModel : ModelBase
    {
        public ObservableCollection<SheduleDay> WeekDays { get; set; }
        public ObservableCollection<Hour> WeekHours { get; set; }
        public double TimeLineOffset { get; set; }
        private TimeSpan StartTime { get; set; }
        private TimeSpan EndTime { get; set; }
        private readonly Timer UpateOffsetTimer;
        public ScheduleMainViewModel()
        {
            this.UpateOffsetTimer = new Timer(CalculateTimeLineOffset);
            WeekDays = new ObservableCollection<SheduleDay>();
            WeekHours = new ObservableCollection<Hour>();
            GetWeek();
        }


        public void GetWeek()
        {
            TimeSpan min_hourtime = TimeSpan.Zero;
            TimeSpan max_hourtime = TimeSpan.Zero;
            int min_hour = 25;
            int max_hour = -1;
            for (DayOfWeek dayOfWeek = DayOfWeek.Monday; dayOfWeek <= DayOfWeek.Friday; dayOfWeek++)
            {
                var schedule = new SheduleDay(Day.GetNearest(dayOfWeek));
                if (!schedule.Class.Any()) { continue; }
                WeekDays.Add(schedule);

                TimeSpan max_newhourtime = schedule.Class.Max(x => x.End);
                TimeSpan min_newhourtime = schedule.Class.Min(x => x.Begin);
                int max_newhour = max_newhourtime.Hours;
                int min_newhour = min_newhourtime.Hours;
                if (max_newhour > max_hour)
                {
                    max_hour = max_newhour;
                    max_hourtime = max_newhourtime;
                }
                if (min_newhour < min_hour)
                {
                    min_hour = min_newhour;
                    min_hourtime = min_newhourtime;
                }
            }

            int j = 0;
            for (int i = min_hour; i <= max_hour; i++, j++)
            {
                this.WeekHours.Add(new Hour(j, i));
            }

            this.StartTime = min_hourtime;
            this.EndTime = max_hourtime;
            CalculateTimeLineOffset(null);
            this.UpateOffsetTimer.Change(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        }

        private void CalculateTimeLineOffset(object obj)
        {
            if (DateTime.Now.TimeOfDay > this.EndTime)
            {
                this.TimeLineOffset = Math.Abs((this.StartTime - this.EndTime).TotalHours) * (Hour.HourHeigth + 4.5);
                this.TimeLineOffset -= 3; //Al final no debe considerar el ultimo margén
            }
            else
            {
                TimeSpan total = DateTime.Now.TimeOfDay-this.StartTime;
                double TotalHours = total.TotalHours;
                this.TimeLineOffset = (TotalHours) * (Hour.HourHeigth + 4.5); //3*1.5 compensa los margenes de 3px entre cuadros de elementos
            }

            this.TimeLineOffset--; //descontar el alto de la boxview misma
            Raise(()=> TimeLineOffset);
        }
    }
}
