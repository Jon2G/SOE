using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;


namespace SchoolOrganizer.Models.TaskFirst
{
    class Agenda
    {
        public string Topic { get; set; }
        public string Duration { get; set; }
        public DateTime Date { get; set; }
        public ObservableCollection<Description> Contenido { get; set; }
        public string Color { get; set; }
        //implementar imagen
    }
}
