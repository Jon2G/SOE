using System;
using Kit.Sql.Attributes;

namespace SchoolOrganizer.Models.Scheduler
{
    public class Subject
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string Group { get;  set; }
        public int IdTeacher { get; set; }

        public Subject() { }
        public Subject(int Id, int IdTeacher, string Name, string Color,string Group)
        {
            this.Id = Id;
            this.IdTeacher = IdTeacher;
            this.Name = Name;
            this.Color = Color;
            this.Group = Group;
        }
    }
}
