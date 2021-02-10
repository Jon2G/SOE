using System;
using System.Collections.Generic;
using System.Text;

namespace OrganizadorEscolar.Widgets.Horario
{
    public class Materia
    {
        public string Horario;
        public string NombreMateria;
        public string Color;
        public Materia(
            string Horario,
            string NombreMateria,
            string Color
            )
        {
            this.Horario = Horario;
            this.NombreMateria = NombreMateria;
            this.Color = Color;
        }
    }
}
