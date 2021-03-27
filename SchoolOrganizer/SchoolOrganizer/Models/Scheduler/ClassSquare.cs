using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolOrganizer.Models.Scheduler
{
    public class ClassSquare
    {
        public double HoursHeigthRequest
        {
            get
            {
                TimeSpan total = End - Begin;
                double TotalHours = total.TotalHours;
                return (TotalHours) * (Hour.HourHeigth+4.5); //3*1.5 compensa los margenes de 3px entre cuadros de elementos
            }
        }
        public string SubjectName { get; set; }
        public string Color { get; internal set; }
        public TimeSpan Begin { get; internal set; }
        public TimeSpan End { get; internal set; }
        public DayOfWeek Day { get; set; }
        public string Group { get; internal set; }
        public string FormattedTime => $"{Begin:hh}:{Begin:mm} - {End:hh}:{End:mm}";
    }
}
