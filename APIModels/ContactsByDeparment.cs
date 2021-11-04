using Newtonsoft.Json;
using SOEWeb.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace SOEWeb.Shared
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
           _Contacts.Add(contact);
        }
    }

}
