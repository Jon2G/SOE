using System;
using System.Collections.Generic;

namespace SchoolOrganizer.Models.Scheduler
{
    public class Day
    {
        public string Name { get; private set; }
        public DayOfWeek DayOfWeek { get; private set; }
        public Day(DayOfWeek DayOfWeek)
        {
            this.DayOfWeek = DayOfWeek;
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
            List<Subject> subjects = new List<Subject>(4);
            TimeSpan begin = TimeSpan.FromHours(14).Add(TimeSpan.FromMinutes(30));
            TimeSpan end = begin.Add(TimeSpan.FromMinutes(90));
            subjects.Add(new Subject(1, "TEORIA DE LA INFORMACION Y CODIFICACION", "#a3ddcb", "6CV1", begin, end));
            begin = end;
            end = begin.Add(TimeSpan.FromMinutes(90));
            subjects.Add(new Subject(2, "TEORIA DE CONTROL DIGITAL", "#e8e9a1", "6CV2", begin, end));
            begin = end;
            end = begin.Add(TimeSpan.FromMinutes(90));
            subjects.Add(new Subject(3, "BASES DE DATOS", "#e6b566", "6CV3", begin, end));
            begin = end;
            end = begin.Add(TimeSpan.FromMinutes(90));
            subjects.Add(new Subject(4, "SISTEMAS DE INFORMACION I", "#e5707e", "6CV4", begin, end));
            return subjects.ToArray();
        }

        public static Day Today()
        {
            return new Day(DateTime.Today.DayOfWeek);
        }
    }
}
