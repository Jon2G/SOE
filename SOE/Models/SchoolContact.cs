using Kit.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOE.Models
{
    public class SchoolContact : ModelBase
    {
        private string _Departament;
        private string _Url;
        private string _Phone;
        private string _Name;

        public string Departament
        {
            get => this._Departament;
            set
            {
                this._Departament = value;
                this.Raise(() => this.Departament);
            }
        }
        public string Name
        {
            get => this._Name;
            set
            {
                this._Name = value;
                this.Raise(() => this.Name);
            }
        }
        public string Phone
        {
            get => this._Phone;
            set
            {
                this._Phone = value;
                this.Raise(() => this.Phone);
            }
        }
        public string Url
        {
            get => this._Url;
            set
            {
                this._Url = value;
                this.Raise(() => this.Url);
            }
        }

        public SchoolContact(string Departament, string Name, string Phone, string Url)
        {
            this.Departament = Departament;
            this.Name = Name;
            this.Phone = Phone;
            this.Url = Url;
        }                   
    }
}
