using System;
using System.Collections.Generic;
using System.Text;

namespace OrganizadorEscolar.Models.Horario
{
    public class Dia
    {
        public DayOfWeek DiaSemana { get;private set; }
        public List<Materia> Materias { get; set; }
        public Dia(DayOfWeek DiaSemana)
        {
            this.DiaSemana = DiaSemana;
        }
        public string GetNombreDia()
        {
            switch (DiaSemana)
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
    }
}
