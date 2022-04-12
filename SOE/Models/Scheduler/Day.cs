using Kit;
using Kit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            this.Name = DayOfWeek.GetDayName();
        }

        public static List<Day> Week()
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


        public async Task<List<ClassSquare>> GetTimeLine(TimeSpan? from = null)
        {
            try
            {
                IEnumerable<ClassTime> classTimes = await ClassTime.IQuery(
                     ClassTime.Collection
                         .WhereEqualsTo(nameof(ClassTime.Day), DayOfWeek.ToString())
                         .OrderBy(nameof(ClassTime.Begin))
                         .OrderBy(nameof(ClassTime.End)));
                List<ClassSquare> classSquares = new List<ClassSquare>();
                ClassTime[] results = (from is null ? classTimes : classTimes.Where(x => x.End > from)).ToArray();
                foreach (ClassTime classTime in results)
                {
                    await classTime.GetSubject();
                    Group group = await classTime.Subject.GetGroup();
                    classSquares.Add(new ClassSquare(classTime.Subject, group, classTime.Begin, classTime.End,
                        classTime.Day));
                }
                return classSquares;
            }
            catch (Exception ex)
            {
                Log.Logger?.Error(ex, "GetTimeLine");
            }
            return await Task.FromResult(new List<ClassSquare>());
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
