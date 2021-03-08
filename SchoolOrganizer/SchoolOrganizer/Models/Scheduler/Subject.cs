using System;
using Kit.Sql.Attributes;

namespace SchoolOrganizer.Models.Scheduler
{
    public class Subject
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get;private set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string Group { get;  set; }
        public TimeSpan Begin { get;  set; }
        public TimeSpan End { get;  set; }
       
        public DayOfWeek Day { get;  set; }

        //public int NDay
        //{
        //    get => (int)Day;
        //    set
        //    {
        //        Day = (DayOfWeek) NDay;
        //    }
        //}

        public Subject() { }
        public Subject(int Id, DayOfWeek Day, string Name, string Color,string Group, TimeSpan Begin, TimeSpan End)
        {
            this.Id = Id;
            this.Day = Day;
            this.Name = Name;
            this.Color = Color;
            this.Group = Group;
            this.Begin = Begin;
            this.End = End;
        }
    }
}
