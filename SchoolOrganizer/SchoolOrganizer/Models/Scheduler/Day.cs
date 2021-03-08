using System;
using System.Collections.Generic;
using SchoolOrganizer.Data;

namespace SchoolOrganizer.Models.Scheduler
{
    public class Day
    {
        public string Name { get; private set; }
        public DateTime Date { get; private set; }
        public DayOfWeek DayOfWeek { get; private set; }
        public bool IsWeekend => (DayOfWeek == DayOfWeek.Sunday || DayOfWeek == DayOfWeek.Saturday);
        public Day(DateTime Date)
        {
            this.Date = Date;
            this.DayOfWeek = Date.DayOfWeek;
            this.Name = GetNameOfDay();
        }
        public string GetNameOfDay()
        {
            switch (DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return "Domingo";
                case DayOfWeek.Monday:
                    return "Lunes";
                case DayOfWeek.Tuesday:
                    return "Martes";
                case DayOfWeek.Wednesday:
                    return "Miércoles";
                case DayOfWeek.Thursday:
                    return "Jueves";
                case DayOfWeek.Friday:
                    return "Viernes";
                case DayOfWeek.Saturday:
                    return "Sábado";
            }
            return string.Empty;
        }
        public IEnumerable<Subject> GetTimeLine()
        {
            return AppData.Instance.LiteConnection.Table<Subject>()
                .Where(x => x.Day == this.DayOfWeek)
                .OrderBy(x => x.Begin).ToList();
        }

        public static Day Today()
        {
            return new Day(DateTime.Today);
        }

        public Day Tommorrow()
        {
            return new Day(Date.AddDays(1));
        }
    }
}
