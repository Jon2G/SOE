using System;
using System.Collections.Generic;
using System.Text;

namespace SOE.Models
{
   public  class Reminders
    {
        public string Title { get; set; }
        public DateTime dateTime { get; set; }
        public TimeSpan timeSpan { get; set; }
        public Reminders(string Title, DateTime dateTime)
        {
            this.Title = Title;
            this.dateTime = dateTime;


        }
    }
}
