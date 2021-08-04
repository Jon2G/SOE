using Kit.Sql.Attributes;
using SOE.Data;
using SOE.Services;
using SOEWeb.Shared;

namespace SOE.Models.Data
{
    public class User :UserBase
    {
        private string _Semester;
        public string Semester
        {
            get => _Semester;
            set
            {
                _Semester = value;
                Raise(() => Semester);
            }
        }

        private string _Name;
      
        private string _Email;
        private string _Career;
        public string Career
        {
            get => _Career;
            set
            {
                _Career = value;
                Raise(() => Career);
            }
        }
        public string Email
        {
            get => _Email;
            set
            {
                _Email = value;
                Raise(() => Email);
            }
        }
        

        private Settings _Settings;
        [Ignore]
        public Settings Settings
        {
            get => _Settings;
            set
            {
                _Settings = value;
                Raise(()=> Settings);
            }
        }

        internal static User Get()
        {
            User User = AppData.Instance.LiteConnection.Table<User>().FirstOrDefault();
            if (User is not null)
            {
                User.School = SchoolService.Get();
            }
            return User;
        }

        public string Name { get => _Name; set { _Name = value; Raise(() => Name); } }

        public bool HasSubjects { get;  set; }

        public User() { }

        public void Save()
        {
            AppData.Instance.LiteConnection.DeleteAll<User>();
            AppData.Instance.LiteConnection.Insert(this);
            SchoolService.Save(this.School);
        }
        public Settings GetSettings()
        {
            var settings = AppData.Instance.LiteConnection
                .Table<Settings>()
                .FirstOrDefault();
            if (settings is null)
            {
                settings = new Settings();
                settings.Save();
            }

            this.Settings = settings;
            return settings;
        }
    }
}
