using System;
using System.Collections.Generic;
using System.Text;

namespace OrganizadorEscolar.Widgets.Tareas
{
    public class Tarea
    {
        public int IdTarea { get; private set; }
        public string Emoji { get; private set; }
        public string NombreMateria { get; private set; }
        public string Descripcion { get; private set; }
        public DateTime FechaEntrega { get; private set; }
        //    public TimeSpan TiempoRestante { get; private set; }
        public int Dias_Restantes { get; private set; }
        public string Color { get; private set; }

        public Tarea(int IdTarea, string NombreMateria, string Descripcion, DateTime FechaEntrega)
        {
            this.IdTarea = IdTarea;
            this.NombreMateria = NombreMateria;
            this.Descripcion = Descripcion;
            this.FechaEntrega = FechaEntrega;
            this.CargarEmoji();
        }
        private void CargarEmoji()
        {
            var TiempoRestante = (this.FechaEntrega - DateTime.Now);
            this.Dias_Restantes = TiempoRestante.Days;

            if (this.Dias_Restantes <= 1)
            {
                this.Color = "#e6000b";
                this.Emoji = "😰";
            }
            else if (this.Dias_Restantes <= 2)
            {
                this.Color = "#db696f";
                this.Emoji = "😨";
            }
            else if (this.Dias_Restantes <= 3)
            {
                this.Color = "#db7669";
                this.Emoji = "😯";
            }
            else if (this.Dias_Restantes <= 4)
            {
                this.Color = "#db8f69";
                this.Emoji = "😪";
            }
            else if (this.Dias_Restantes <= 5)
            {
                this.Color = "#f0a986";
                this.Emoji = "😧";
            }
            else if (this.Dias_Restantes <= 6)
            {
                this.Color = "#f0c286";
                this.Emoji = "😦";
            }
            else if (this.Dias_Restantes <= 7)
            {
                this.Color = "#f0ce86";
                this.Emoji = "😕";
            }
            else if (this.Dias_Restantes <= 8)
            {
                this.Color = "#f0e486";
                this.Emoji = "🤨";
            }
            else if (this.Dias_Restantes <= 9)
            {
                this.Color = "#d0f086";
                this.Emoji = "😶";
            }
            else if (this.Dias_Restantes <= 10)
            {
                this.Color = "#b6f086";
                this.Emoji = "😐";
            }
            else if (this.Dias_Restantes <= 11)
            {
                this.Color = "#a9f086";
                this.Emoji = "😅";
            }
            else
            {
                this.Color = "#fcfdff";
                this.Emoji = "😌";
            }
        }
    }
}
