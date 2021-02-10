using OrganizadorEscolar.Models.Horario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace OrganizadorEscolar.Widgets.Tareas
{
    public abstract class WidgetTareas
    {
        public abstract WidgetTareas Refresh();
        public abstract bool IsItentDataSet();
        public abstract IntentData GetIntentData();
        public List<Tarea> Tareas { get; protected set; }
        public static WidgetTareas Instance { get; protected set; }
        public DateTime DiaActual { get; protected set; }

        public WidgetTareas()
        {
            Tareas = new List<Tarea>();
        }
        public static void Init(WidgetTareas WidgetHorario)
        {
            Instance = WidgetHorario;
            Instance.Cargar();
        }

        public void Cargar()
        {
            this.DiaActual = DateTime.Now;
            Tareas.Clear();
            string[] materias = new string[] { "Ingenieria de Software", "Sistemas Operativos", "Modulacion Digital", "Bases de datos", "Arquitectura de computadoras" };

            string[] Descripcion = new string[] { "Exposición", "Ensayo 3 páginas", "Práctica 5", "Actividad edmodo", "Investigación computadoras" };

            Random random = new Random();
            int j = random.Next(0, 4);
            DateTime entrega = DateTime.Now;
            for (int i = 0; i < 30; i++)
            {
                entrega = entrega.AddDays(1);
                Tareas.Add(new OrganizadorEscolar.Widgets.Tareas.Tarea(i, materias[(j + i) % 5], Descripcion[(j + i) % 5], entrega));
            }
            this.Tareas = Tareas.OrderBy(x => x.Dias_Restantes).ToList();
        }

    }
}
