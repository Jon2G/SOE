using Kit.Sql.Attributes;
using Kit.Sql.Interfaces;
using System;

namespace SOEWeb.Shared
{
    public class Subject : IGuid
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string ColorDark { get; set; }
        public string Group { get; set; }
        public int IdTeacher { get; set; }
        public Guid Guid { get; set; }

        public Subject() 
        {

        }
        public Subject(int Id, int IdTeacher, string Name, string Color,string ColorDark, string Group)
        {
            this.Id = Id;
            this.IdTeacher = IdTeacher;
            this.Name = Name;
            this.Color = Color;
            this.ColorDark = ColorDark;
            this.Group = Group;
        }


    }
}
