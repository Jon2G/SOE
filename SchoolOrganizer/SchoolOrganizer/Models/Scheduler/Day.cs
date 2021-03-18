using System;
using System.Collections.Generic;
using System.Linq;
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

        string GetNameOfDay()
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
        public List<ClassSquare> GetTimeLine()
        {
            List<ClassSquare> classSquares = new List<ClassSquare>();

            foreach (ClassTime classTime in AppData.Instance.LiteConnection.Table<ClassTime>()
                .Where(x => x.Day == this.DayOfWeek)
                .OrderBy(x => x.Begin))
            {
                Subject subject = AppData.Instance.LiteConnection.Find<Subject>(classTime.IdSubject);
                classSquares.Add(new ClassSquare()
                {
                    SubjectName = subject.Name,
                    Color = subject.Color,
                    Begin = classTime.Begin,
                    End = classTime.End,
                    Day = classTime.Day,
                    Group = subject.Group
                });
            }
            return classSquares;
        }

        public static Day Today()
        {
            Day Day = new Day(DateTime.Today); ;
            do
            {
                Day = new Day(Day.Date.AddDays(1));
            } while (Day.IsWeekend);

            return Day;
        }

        public Day Tommorrow()
        {
            Day Day=this;
            do
            {
                Day = new Day(Day.Date.AddDays(1));
            } while (Day.IsWeekend);

            return Day;
        }
        public Day Yesterday()
        {
            Day Day = this;
            do
            {
                Day = new Day(Day.Date.AddDays(-1));
            } while (Day.IsWeekend);
            return Day;
        }
    }
}
