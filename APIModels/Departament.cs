﻿using Kit.Model;
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
    }  
}