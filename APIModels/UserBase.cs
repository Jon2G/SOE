using Kit.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Kit.Sql.Attributes;

namespace APIModels
{
    public class UserBase : ModelBase
    {
        private string _Boleta;
        [PrimaryKey, MaxLength(10)]
        public string Boleta { get => _Boleta; set { _Boleta = value; Raise(() => Boleta); } }
        private string _Password;
        public string Password { get => _Password; set { _Password = value; Raise(() => Password); } }
        [Ignore]
        public School School
        {
            get => _School;
            set
            {
                _School = value;
                Raise(() => School);
            }
        }
        private School _School;
    }
}
