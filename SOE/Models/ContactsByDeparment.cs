using System.Collections.Generic;
using System.Threading.Tasks;

namespace SOE.Models
{
    public class ContactsByDeparment
    {
        public string Departament { get; set; }
        public SchoolContact[] Contacts
        {
            get => this._Contacts.ToArray();
            set
            {
                this._Contacts = new List<SchoolContact>(value);
            }
        }
        private List<SchoolContact> _Contacts;
        public ContactsByDeparment(string departament)
        {
            this.Departament = departament;
            this._Contacts = new List<SchoolContact>();

        }

        public void Add(SchoolContact contact)
        {
            this._Contacts.Add(contact);
        }

        public static Task<List<ContactsByDeparment>> GetByDepartment()
        {
            return SchoolContact.GetAll().ContinueWith(t =>
             {
                 IEnumerable<SchoolContact>? contacts = t.Result;
                 string lastDepartment = string.Empty;
                 List<ContactsByDeparment> contactsByDeparment = new List<ContactsByDeparment>();
                 ContactsByDeparment byDeparment = null;
                 foreach (SchoolContact schoolContact in contacts)
                 {
                     if (lastDepartment != schoolContact.Departament)
                     {
                         lastDepartment = schoolContact.Departament;
                         byDeparment = new ContactsByDeparment(lastDepartment);
                         contactsByDeparment.Add(byDeparment);
                     }
                     byDeparment?.Add(schoolContact);
                 }
                 return contactsByDeparment;
             });
        }
    }

}
