using Kit.Model;
using Kit.Sql.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOEWeb.Shared
{
    [Serializable]
    public class Departament : ModelBase, IGuid
    {
        public Guid Guid { get; set; }

        private string _Name;
        public string Name
        {
            get => this._Name;
            set
            {
                this._Name = value;
                this.Raise(() => this.Name);
            }
        }

        public bool IsValid() => !string.IsNullOrEmpty(Name);

        public Departament()
        {

        }

        public static bool operator ==(Departament point1, Departament point2) => point1?.Equals(point2) ?? point2 is null;

        public static bool operator !=(Departament point1, Departament point2) => !(point1 == point2);

        public override bool Equals(object obj)
        {
            if (obj is Departament departament)
            {
                return (departament.Guid != Guid.Empty && departament.Guid == this.Guid) 
                       || departament.Name == this.Name;
            }
            return false;
        }
    }
}