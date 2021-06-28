using System;
using System.Collections.Generic;
using System.Text;
using Kit.Sql.Attributes;

namespace SchoolOrganizer.Models.Scheduler
{
    public class ClassTime
    {
        [PrimaryKey, AutoIncrement] 
        public int Id { get; set; }
        [NotNull]
        public int IdSubject { get; set; }
        public string Group { get; set; }
        public DayOfWeek Day { get; set; }
        public TimeSpan Begin { get; set; }
        public TimeSpan End { get; set; }

        public ClassTime()
        {

        }

        public ClassTime(int Id, string Group, int IdSubject, DayOfWeek Day, TimeSpan Begin, TimeSpan End)
        {
            this.Id = Id;
            this.Group = Group;
            this.Day = Day;
            this.IdSubject = IdSubject;
            this.Begin = Begin;
            this.End = End;
        }
    }
}
