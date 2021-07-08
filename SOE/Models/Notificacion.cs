using System;
using System.Collections.Generic;
using System.Text;

namespace SOE.Models
{
    public class Notificacion
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Unread { get; set; }
        //public string Image { get; set; }
        public Notificacion(string Title, string Description,bool Unread=true)
        {
            this.Title = Title;
            this.Description = Description;
            this.Unread = Unread;
        }
    }
}
