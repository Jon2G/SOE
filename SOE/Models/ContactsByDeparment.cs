using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SOE.Models
{
    [Serializable]
    public class ContactsByDeparment 
    {
        [JsonProperty("Departament")]
        public Departament Departament { get;  set; }

        [JsonProperty("Contacts")]
        public SchoolContact[] Contacts
        {
            get=> this._Contacts.ToArray();
            set
            {
                this._Contacts = new List<SchoolContact>(value);
            }
        }
        private List<SchoolContact> _Contacts;
        public ContactsByDeparment(Departament Departament):this()
        {
            this.Departament = Departament;
            
        }

        public ContactsByDeparment()
        {
            this._Contacts = new List<SchoolContact>();
        }

        public void Add(SchoolContact contact)
        {
           this._Contacts.Add(contact);
        }
    }

}
