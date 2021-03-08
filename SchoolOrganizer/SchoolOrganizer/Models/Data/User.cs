using System;
using System.Collections.Generic;
using System.Text;
using Kit;
using Kit.Model;
using Kit.Sql.Attributes;

namespace SchoolOrganizer.Models.Data
{
    public class User : ModelBase
    {
        private string _Boleta;
        private string _Password;
        private bool _RemeberMe;
        private string _Name;
        [PrimaryKey, MaxLength(10)]
        public string Boleta { get => _Boleta; set { _Boleta = value; Raise(() => Boleta); } }
        public string Password { get => _Password; set { _Password = value; Raise(() => Password); } }
        public bool RemeberMe {
            get => _RemeberMe;
            set
            {
                _RemeberMe = value;
                Raise(() => RemeberMe);
            }
        }
        public string Name { get => _Name; set { _Name = value; Raise(() => Name); } }
        [Ignore]
        public bool IsLogedIn { get; set; }

        public User() { }
        public User(string Boleta, string Password, bool RemeberMe) { }
    }
}
