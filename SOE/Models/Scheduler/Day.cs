using System;
using System.Collections.Generic;
using SOEWeb.Shared;
using Kit.Model;
using SOE.Data;
using Kit;
namespace SOE.Models.Scheduler
{
    public class Day : ModelBase
    {
        public string Name { get; private set; }
        public string ShortName => Name.Substring(0, 3);
        public DateTime Date { get; private set; }
        public DayOfWeek DayOfWeek { get; private set; }
        public bool IsWeekend => (DayOfWeek == DayOfWeek.Sunday || DayOfWeek == DayOfWeek.Saturday);

        public Day(DateTime Date)
        {
            this.Date = Date;
            this.DayOfWeek = Date.DayOfWeek;
            this.Name = GetNameOfDay();
        }

        internal static List<Day> Week()
        {
            return new List<Day>()
            {
                GetNearest(DayOfWeek.Monday),
                GetNearest(DayOfWeek.Tuesday),
                GetNearest(DayOfWeek.Wednesday),
                GetNearest(DayOfWeek.Thursday),
                GetNearest(DayOfWeek.Friday)
            };
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
            try
            {
                if (AppData.Instance is null)
                {
                    AppData.Init();
                }
                TimeSpan EndTime = TimeSpan.Zero;
                foreach (ClassTime classTime in AppData.Instance.LiteConnection.Table<ClassTime>()
                    .Where(x => x.Day == this.DayOfWeek)
                    .OrderBy(x => x.Begin))
                {
                    Subject subject = AppData.Instance.LiteConnection.Find<Subject>(classTime.IdSubject);
                    classSquares.Add(new ClassSquare(subject, classTime.Begin, classTime.End, classTime.Day));
                }
            }
            catch (Exception ex)
            {
                Log.Logger?.Error(ex, "GetTimeLine");
            }

            return classSquares;
        }

        public static Day Today()
        {
            Day Day = new Day(DateTime.Today);
            while (Day.IsWeekend)
            {
                Day = new Day(Day.Date.AddDays(1));
            }

            return Day;
        }

        public Day Tommorrow()
        {
            Day Day = this;
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

        public static Day GetNearest(DayOfWeek day) => new Day(day.GetNearest());
    }
}
