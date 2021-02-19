using SchoolOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolOrganizer.ViewModels.ViewItems
{
    public class PinnedTasksViewModel
    {
        public List<Card> Tasks { get; set; }
        public PinnedTasksViewModel()
        {
        }
    }
}
