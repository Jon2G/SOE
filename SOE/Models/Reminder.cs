using System;
using System.Collections.Generic;
using System.Text;

namespace SOE.Models
{
   public  class Reminder
    {
        public string Title { get; set; }
        public DateTime DateTime { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public Reminder(string Title, DateTime dateTime)
        {
            this.Title = Title;
            this.DateTime = dateTime;


        }
    }
}
