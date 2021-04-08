using Kit.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using SchoolOrganizer.Models.Scheduler;


namespace SchoolOrganizer.Models.TaskFirst
{
    public class ToDo : ModelBase
    {
       
        private string n_Tarea;
        private string _H_Entrga;
        private DateTime date;
        private Subject subject;
        private string description;

        public string N_Tarea
        {
            get => n_Tarea;
            set
            {
                n_Tarea = value;
                Raise(() => N_Tarea);
            }
        }
        public string H_Entrga
        {
            get => _H_Entrga;
            set
            {
                _H_Entrga = value;
                Raise(() => H_Entrga);
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
        public Subject Subject
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
        //implementar imagen

        public ToDo()
        {
            N_Tarea = "";
            H_Entrga = "11:00";
            Date = DateTime.Now;
            //Subject = null;
            Description = "";
        }
        
    }
}
