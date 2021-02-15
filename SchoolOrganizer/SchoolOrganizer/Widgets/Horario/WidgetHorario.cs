using System;
using System.Collections.Generic;
using SchoolOrganizer.Models.Scheduler;

namespace SchoolOrganizer.Widgets.Horario
{
    public abstract class WidgetHorario
    {
        public abstract WidgetHorario Refresh();
        public abstract bool IsItentDataSet();
        public abstract IntentData GetIntentData();
        public List<Materia> Materias { get; protected set; }
        public static WidgetHorario Instance { get; protected set; }

        public HorarioSemanalModel Model { get; protected set; }
        public Day Actual { get; protected set; }
        public DateTime DiaActual { get; protected set; }

        public WidgetHorario()
        {
            Materias = new List<Materia>();
        }
        public static void Init(WidgetHorario WidgetHorario)
        {
            Instance = WidgetHorario;

            Instance.DiaActual = DateTime.Now;
            Instance.Model = new HorarioSemanalModel(false, false);
            Instance.GetDay();
            Instance.CargarDia(Instance.Actual);
        }

        public void CargarDia(Day actual)
        {
            Materias.Clear();
            string[] materias = new string[] { "Ingenieria de Software", "Sistemas Operativos", "Modulacion Digital", "Bases de datos", "Arquitectura de computadoras" };

            string[] colores = new string[] { "#b9abff", "#acf2c8", "#ede19f", "#9dbfeb", "#de998e" };

            string[] horarios = new string[] { "14:30 - 16:00", "16:00 - 17:30", "17:30 - 19:00", "19:00 - 20:30", "20:30 - 22:00" };


            Random random = new Random();
            int j = random.Next(0, 4);
            for (int i = 0; i < 5; i++)
            {
                Materias.Add(new Materia(horarios[i], materias[(j + i) % 5], colores[(j + i) % 5]));
            }
            //Materias.Add(new OrganizadorEscolar.Widgets.Horario.Materia("14:30 - 16:00", "Ingenieria de Software", "#b9abff"));
            //Materias.Add(new OrganizadorEscolar.Widgets.Horario.Materia("16:00 - 17:30", "Sistemas Operativos", "#acf2c8"));
            //Materias.Add(new OrganizadorEscolar.Widgets.Horario.Materia("17:30 - 19:00", "Modulacion Digital", "#ede19f"));
            //Materias.Add(new OrganizadorEscolar.Widgets.Horario.Materia("19:00 - 20:30", "Bases de datos", "#9dbfeb"));
            //Materias.Add(new OrganizadorEscolar.Widgets.Horario.Materia("20:30 - 22:00", "Arquitectura de computadoras", "#de998e"));
        }
        public void Ayer()
        {
            AvanzarDias(-1);
            CargarDia(this.Actual);
        }
        public void Tomorrow()
        {
            AvanzarDias(1);
            CargarDia(this.Actual);
        }
        private void AvanzarDias(int Dias)
        {
            DiaActual = DiaActual.AddDays(Dias); //Retroceder a la fecha de ayer
            if (!Model.IncluirSabado && DiaActual.DayOfWeek == DayOfWeek.Saturday)
            {
                AvanzarDias(Dias);
            }
            if (!Model.IncluirDomingo && DiaActual.DayOfWeek == DayOfWeek.Sunday)
            {
                AvanzarDias(Dias);
            }
            Actual = Model.GetDay(DiaActual.DayOfWeek);
        }
        private void GetDay()
        {
            if (!Model.IncluirSabado && DiaActual.DayOfWeek == DayOfWeek.Saturday)
            {
                AvanzarDias(1);
                return;
            }
            if (!Model.IncluirDomingo && DiaActual.DayOfWeek == DayOfWeek.Sunday)
            {
                AvanzarDias(1);
                return;
            }
            Actual = Model.GetDay(DiaActual.DayOfWeek);
        }
    }
}
