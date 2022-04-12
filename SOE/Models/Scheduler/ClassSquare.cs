using Kit;
using System;
namespace SOE.Models.Scheduler
{
    public class ClassSquare
    {
        public double HoursHeigthRequest
        {
            get
            {
                TimeSpan total = End - Begin;
                double TotalHours = total.TotalHours;
                return (TotalHours) * (Hour.HourHeigth + 4.5); //3*1.5 compensa los margenes de 3px entre cuadros de elementos
            }
        }
        public Subject Subject { get; set; }
        public Group Group { get; set; }
        public TimeSpan Begin { get; internal set; }
        public TimeSpan End { get; internal set; }
        public DayOfWeek Day { get; set; }
        public string DayName => Day.GetDayName();
        public string FormattedTime => $"{Begin:hh}:{Begin:mm} - {End:hh}:{End:mm}";
        public TimeSpan Duration => End - Begin;

        public ClassSquare(Subject Subject, Group group, TimeSpan Begin, TimeSpan End, DayOfWeek Day)
        {
            this.Subject = Subject;
            this.Group = group;
            this.Begin = Begin;
            this.End = End;
            this.Day = Day;
        }
    }
}
