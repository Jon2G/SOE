using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrganizadorEscolar.Models.Horario
{
    public class HorarioSemanalModel
    {
        public readonly bool IncluirSabado;
        public readonly bool IncluirDomingo;
        public readonly Dia[] Dias;
        public HorarioSemanalModel(bool IncluirSabado = false, bool IncluirDomingo = false)
        {
            this.IncluirSabado = IncluirSabado;
            this.IncluirDomingo = IncluirDomingo;
            List<Dia> Dias = new List<Dia>(7);
            if (IncluirDomingo)
            {
                Dias.Add(new Dia(DayOfWeek.Sunday));
            }
            Dias.Add(new Dia(DayOfWeek.Monday));
            Dias.Add(new Dia(DayOfWeek.Tuesday));
            Dias.Add(new Dia(DayOfWeek.Wednesday));
            Dias.Add(new Dia(DayOfWeek.Thursday));
            Dias.Add(new Dia(DayOfWeek.Friday));
            if (IncluirSabado)
            {
                Dias.Add(new Dia(DayOfWeek.Saturday));
            }
            this.Dias = Dias.ToArray();
        }

        public Dia GetDay(DayOfWeek DayOfWeek)
        {
            return this.Dias.FirstOrDefault(x => x.DiaSemana == DayOfWeek);
        }
    }
}
