using System;
using System.Collections.Generic;
using System.Text;
using Kit.Model;
using SchoolOrganizer.Models.Scheduler;

namespace SchoolOrganizer.ViewModels.ViewItems.ScheduleViewModel
{
    public class SheduleDay: ModelBase
    {
        public Day Day { get; set; }
        public IEnumerable<ClassSquare> Class { get; set; }

        public SheduleDay(Day Day)
        {
            this.Day = Day;
            this.Class = Day.GetTimeLine();
        }
    }
}
