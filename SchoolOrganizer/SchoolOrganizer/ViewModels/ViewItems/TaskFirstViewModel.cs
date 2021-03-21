using SchoolOrganizer.Models.Scheduler;
using SchoolOrganizer.Models.TaskFirst;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SchoolOrganizer.ViewModels.ViewItems
{
    class TaskFirstViewModel
    {
        Subject subject = new Subject();
        public ObservableCollection<Agenda> MyAgenda { get => GetAgenda(); }

        private ObservableCollection<Agenda> GetAgenda()
        {
           
            return new ObservableCollection<Agenda>
            {
                new Agenda { Topic = "Diagrama", Duration = "11:30 ", Color = "#B96CBD", Date = new DateTime(2020, 3, 23),
                    Contenido = new ObservableCollection<Description>{ new Description { Name = "Base de Datos",Descripción="Diagrama sobre una tienda fisica"} } },

                new Agenda { Topic = "Proyecto", Duration = "07:30 UTC - 11:30 UTC", Color = "#49A24D", Date = new DateTime(2020, 3, 24),
                    Contenido = new ObservableCollection<Description>{ new Description { Name = "Sistemas De Información"} } },

                new Agenda { Topic = "Nuevas evidencias", Duration = "07:30 UTC - 11:30 UTC", Color = "#FDA838", Date = new DateTime(2020, 3, 25),
                    Contenido = new ObservableCollection<Description>{ new Description { Name = "Administracion" }} },

                new Agenda { Topic = "Practica2", Duration = "07:30 UTC - 11:30 UTC", Color = "#F75355", Date = new DateTime(2020, 3, 26),
                    Contenido = new ObservableCollection<Description>{ new Description { Name = "Nuevas Tecnologias" } } },

                new Agenda { Topic = "Practica3", Duration = "07:30 UTC - 11:30 UTC", Color = "#00C6AE", Date = new DateTime(2020, 3, 27),
                    Contenido = new ObservableCollection<Description>{ new Description { Name = "Control Digital"}} },

                new Agenda { Topic = "Pagina para SuperEspias", Duration = "07:30 UTC - 11:30 UTC", Color = "#455399", Date = new DateTime(2020, 3, 28),
                    Contenido = new ObservableCollection<Description>{ new Description { Name = "Tecnologias" } } },
                new Agenda { Topic = "Pagina para SuperEspias", Duration = "07:30 UTC - 11:30 UTC", Color = "#455399", Date = new DateTime(2020, 3, 28),
                    Contenido = new ObservableCollection<Description>{ new Description { Name = "Tecnologias" } } },
                new Agenda { Topic = "Pagina para SuperEspias", Duration = "07:30 UTC - 11:30 UTC", Color = "#455399", Date = new DateTime(2020, 3, 28),
                    Contenido = new ObservableCollection<Description>{ new Description { Name = "Tecnologias" } } },
                new Agenda { Topic = "Pagina para SuperEspias", Duration = "07:30 UTC - 11:30 UTC", Color = "#455399", Date = new DateTime(2020, 3, 28),
                    Contenido = new ObservableCollection<Description>{ new Description { Name = "Tecnologias" } } }
            };
        }
    }
}
