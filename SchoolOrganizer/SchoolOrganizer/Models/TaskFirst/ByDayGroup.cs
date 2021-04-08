﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SchoolOrganizer.Models.TaskFirst
{
    public class ByDayGroup
    {
        public DateTime FDateTime { get; set; }
        public string Month { get => Kit.Extensions.Helpers.Mes(FDateTime.Month); }
        public ObservableCollection<BySubjectGroup> SubjectGroups { get; set; }
        public int Tareas => SubjectGroups.Sum(x => x.ToDoS.Count);

        public ByDayGroup()
        {
            this.SubjectGroups = new ObservableCollection<BySubjectGroup>();
        }
    }
}
