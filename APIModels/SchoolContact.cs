using Kit.Model;
using Kit.Sql.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOEWeb.Shared
{
    public class SchoolContact : ModelBase,IGuid
    {
        
        private string _Url;
        private string _Phone;
        private string _Name;
        private string _Correo;
        public Guid Guid { get; set; }

        private Departament _Departament;
        public Departament Departament
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
        public string Correo
        {
            get => this._Correo;
            set
            {
                this._Correo = value;
                this.Raise(() => this.Correo);
            }
        }
        public bool IsOwner
        {
            get;set;
        }

        public SchoolContact(Departament Departament, string Name, string Phone, string Url,string Correo)
        {
            this.Departament = Departament;
            this.Name = Name;
            this.Phone = Phone;
            this.Url = Url;
            this.Correo = Correo;
        }
        public SchoolContact()
        {

        }
    }
}
