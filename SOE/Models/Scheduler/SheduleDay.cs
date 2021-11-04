using System.Collections.Generic;
using Kit.Model;

namespace SOE.Models.Scheduler
{
    public class SheduleDay: ModelBase
    {
        public Day Day { get; set; }
        public List<ClassSquare> Class { get; set; }


        public SheduleDay(Day Day)
        {
            this.Day = Day;
            this.Class = Day.GetTimeLine();
        }
    }
}
