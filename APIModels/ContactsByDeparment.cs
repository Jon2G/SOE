using SOEWeb.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SOEWeb.Shared
{
    public class ContactsByDeparment : ObservableCollection<SchoolContact>
    {
        public Departament Departament { get;  set; }
        public ContactsByDeparment(Departament Departament)
        {
            this.Departament = Departament;
        }

        public ContactsByDeparment()
        {
            
        }
    }

}
