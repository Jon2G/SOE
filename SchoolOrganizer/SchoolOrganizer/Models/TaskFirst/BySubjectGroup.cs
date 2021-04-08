using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using SchoolOrganizer.Models.Scheduler;

namespace SchoolOrganizer.Models.TaskFirst
{
   public class BySubjectGroup
    {
        public  Subject Subject { get; set; }
        public ObservableCollection<ToDo> ToDoS { get; set; }

        public BySubjectGroup(Subject Subject)
        {
            this.Subject = Subject;
            ToDoS = new ObservableCollection<ToDo>();
        }
    }
}
