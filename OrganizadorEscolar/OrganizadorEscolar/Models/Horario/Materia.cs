using System;
using System.Collections.Generic;
using System.Text;

namespace OrganizadorEscolar.Models.Horario
{
    public class Materia
    {
        public string Nombre { get;private set; }
        public string Salon { get; private set; }
        public string Grupo { get; private set; }
        public TimeSpan Inicio { get; private set; }
        public TimeSpan Fin { get; private set; }

        public Materia(string Nombre ,string Salon,string Grupo ,TimeSpan Inicio,TimeSpan Fin)
        {
            this.Nombre = Nombre;
            this.Salon = Salon;
            this.Grupo = Grupo;
            this.Inicio = Inicio;
            this.Fin = Fin;
        }
    }
}
