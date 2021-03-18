using SchoolOrganizer.Models.TaskFirst;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SchoolOrganizer.ViewModels.ViewItems
{
    class TaskFirstViewModel
    {
       
        public ObservableCollection<Agenda> MyAgenda { get => GetAgenda(); }

        private ObservableCollection<Agenda> GetAgenda()
        {
           
            return new ObservableCollection<Agenda>
            {
                new Agenda { Topic = "Reporte de Matematicas", Duration = "07:30 UTC - 11:30 UTC", Color = "#B96CBD", Date = new DateTime(2020, 3, 23),
                    Speakers = new ObservableCollection<Description>{ new Description { Name = "Maddy Leger", Time = "07:30" }, new Description { Name = "David Ortinau", Time = "08:30" }, new Description { Name = "Gerald Versluis", Time = "10:30" } } },

                new Agenda { Topic = "Proyecto", Duration = "07:30 UTC - 11:30 UTC", Color = "#49A24D", Date = new DateTime(2020, 3, 24),
                    Speakers = new ObservableCollection<Description>{ new Description { Name = "Maddy Leger", Time = "07:30" }, new Description { Name = "David Ortinau", Time = "08:30" }, new Description { Name = "Gerald Versluis", Time = "10:30" } } },

                new Agenda { Topic = "Nuevas evidencias", Duration = "07:30 UTC - 11:30 UTC", Color = "#FDA838", Date = new DateTime(2020, 3, 25),
                    Speakers = new ObservableCollection<Description>{ new Description { Name = "Maddy Leger", Time = "07:30" }, new Description { Name = "David Ortinau", Time = "08:30" }, new Description { Name = "Gerald Versluis", Time = "10:30" } } },

                new Agenda { Topic = "Xamarin Productivity to the Max", Duration = "07:30 UTC - 11:30 UTC", Color = "#F75355", Date = new DateTime(2020, 3, 26),
                    Speakers = new ObservableCollection<Description>{ new Description { Name = "Maddy Leger", Time = "07:30" }, new Description { Name = "David Ortinau", Time = "08:30" }, new Description { Name = "Gerald Versluis", Time = "10:30" } } },

                new Agenda { Topic = "All Things Xamarin.Forms Shell", Duration = "07:30 UTC - 11:30 UTC", Color = "#00C6AE", Date = new DateTime(2020, 3, 27),
                    Speakers = new ObservableCollection<Description>{ new Description { Name = "Maddy Leger", Time = "07:30" }, new Description { Name = "David Ortinau", Time = "08:30" }, new Description { Name = "Gerald Versluis", Time = "10:30" } } },

                new Agenda { Topic = "Building Beautiful Apps", Duration = "07:30 UTC - 11:30 UTC", Color = "#455399", Date = new DateTime(2020, 3, 28),
                    Speakers = new ObservableCollection<Description>{ new Description { Name = "Maddy Leger", Time = "07:30" }, new Description { Name = "David Ortinau", Time = "08:30" }, new Description { Name = "Gerald Versluis", Time = "10:30" } } }
            };
        }
    }
}
