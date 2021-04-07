using System;
using System.Collections.Generic;
using System.Text;
using Kit;
using Kit.Model;
using Kit.Sql.Attributes;
using SchoolOrganizer.Data;
using SchoolOrganizer.Saes;

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
        [Ignore]
        public School School { get; set; }
        public string HomePage
        {
            get => School?.HomePage;
            set
            {
                if (School is null)
                {
                    School = new School(HomePage, String.Empty, String.Empty);
                }
            }
        }
        internal static User Get()
        {

            return AppData.Instance.LiteConnection.Table<User>().FirstOrDefault();
        }

        public bool RemeberMe
        {
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
