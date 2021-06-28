using System;
using System.Collections.Generic;
using System.Linq;

namespace SOE.Models.Scheduler
{
    public class HorarioSemanalModel
    {
        public readonly bool IncluirSabado;
        public readonly bool IncluirDomingo;
        public readonly Day[] Dias;
        public HorarioSemanalModel(bool IncluirSabado = false, bool IncluirDomingo = false)
        {
            this.IncluirSabado = IncluirSabado;
            this.IncluirDomingo = IncluirDomingo;
            List<Day> Dias = new List<Day>(7);
            //if (IncluirDomingo)
            //{
            //    Dias.Add(new Day(DayOfWeek.Sunday));
            //}
            //Dias.Add(new Day(DayOfWeek.Monday));
            //Dias.Add(new Day(DayOfWeek.Tuesday));
            //Dias.Add(new Day(DayOfWeek.Wednesday));
            //Dias.Add(new Day(DayOfWeek.Thursday));
            //Dias.Add(new Day(DayOfWeek.Friday));
            //if (IncluirSabado)
            //{
            //    Dias.Add(new Day(DayOfWeek.Saturday));
            //}
            this.Dias = Dias.ToArray();
        }

        public Day GetDay(DayOfWeek DayOfWeek)
        {
            return this.Dias.FirstOrDefault(x => x.DayOfWeek == DayOfWeek);
        }
    }
}
