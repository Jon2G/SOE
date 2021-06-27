using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Kit.Sql.Attributes;

namespace APIModels
{
    public class ClassTime
    {
        [PrimaryKey, AutoIncrement] 
        public int Id { get; set; }
        [NotNull]
        public int IdSubject { get; set; }
        public string Group { get; set; }
        public DayOfWeek Day { get; set; }

        [XmlIgnore]
        public TimeSpan Begin { get; set; }
        [Ignore]
        public long BeginTicks
        {
            get => Begin.Ticks;
            set => Begin = TimeSpan.FromTicks(value);
        }
        [XmlIgnore]
        public TimeSpan End { get; set; }
        [Ignore]
        public long EndTicks
        {
            get=>End.Ticks;
            set => End = TimeSpan.FromTicks(value);
        }

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
