using Kit.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;


namespace SchoolOrganizer.Models.TaskFirst
{
    public class ToDo : ModelBase
    {
       
        private string n_Tarea;
        private string duration;
        private DateTime date;
        private string subject;
        private string description;
        private string color;
        public string N_Tarea
        {
            get => n_Tarea;
            set
            {
                n_Tarea = value;
                Raise(() => N_Tarea);
            }
        }
        public string Duration
        {
            get => duration;
            set
            {
                duration = value;
                Raise(() => Duration);
            }
        }
        public DateTime Date
        {
            get => date;
            set
            {
                date = value;
                Raise(() => Date);
            }
        }
        public string Subject
        {
            get => subject;
            set
            {
                subject = value;
                Raise(() => Subject);
            }
        }
        public string Description
        {
            get => description;
            set
            {
                description = value;
                Raise(() => Description);
            }
        }
        public string Color
        {
            get => color;
            set
            {
                color = value;
                Raise(() => Color);
            }
        }
        //implementar imagen

        public ToDo()
        {
            N_Tarea = "";
            Duration = "11:00";
            Date = DateTime.Now;
            Subject = "Base de datos";
            Description = "";
            Color = "#B96CBD";
         }
        
    }
}
