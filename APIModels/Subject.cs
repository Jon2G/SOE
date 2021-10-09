using Kit.Sql.Attributes;
using Kit.Sql.Interfaces;
using Newtonsoft.Json;
using SOEWeb.Shared.Interfaces;
using System;

namespace SOEWeb.Shared
{
    [Preserve]
    public class Subject : IGuid, IOffline
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        [Ignore, JsonIgnore]
        public ThemeColor ThemeColor { get; set; }

        public string Color
        {
            get => ThemeColor?.Light;
            set
            {
                if (ThemeColor is null)
                {
                    ThemeColor = new ThemeColor() { Light = value };
                }
                else
                {
                    ThemeColor.Light = value;
                }
            }
        }
        public string ColorDark
        {
            get => ThemeColor?.Dark;
            set
            {
                if (ThemeColor is null)
                {
                    ThemeColor = new ThemeColor() { Dark = value };
                }
                else
                {
                    ThemeColor.Dark = value;
                }
            }
        }


        public string Group { get; set; }
        public int GroupId { get; set; }
        public int IdTeacher { get; set; }
        public Guid Guid { get; set; }
        public bool IsOffline { get; set; }

        public Subject()
        {

        }
        public Subject(int Id, int IdTeacher, string Name, ThemeColor color, string Group)
        {
            this.Id = Id;
            this.IdTeacher = IdTeacher;
            this.Name = Name;
            this.ThemeColor = color;
            this.Group = Group;
        }


    }
}
