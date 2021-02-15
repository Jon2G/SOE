using System;

namespace SchoolOrganizer.Models.Scheduler
{
    public class Subject
    {
        public int Id { get;private set; }
        public string Name { get;private set; }
        public string Color { get;private set; }
        public string Group { get; private set; }
        public TimeSpan Begin { get; private set; }
        public TimeSpan End { get; private set; }

        public Subject(int Id, string Name, string Color,string Group, TimeSpan Begin, TimeSpan End)
        {
            this.Id = Id;
            this.Name = Name;
            this.Color = Color;
            this.Group = Group;
            this.Begin = Begin;
            this.End = End;
        }
    }
}
