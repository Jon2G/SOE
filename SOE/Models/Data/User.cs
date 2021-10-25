using Kit.Sql.Attributes;
using SOE.API;
using SOE.Data;
using SOE.Services;
using SOEWeb.Shared;
using SOEWeb.Shared.Interfaces;

namespace SOE.Models.Data
{
    public class User : UserBase
    {
        private Settings _Settings;
        [Ignore]
        public Settings Settings
        {
            get => _Settings;
            set
            {
                _Settings = value;
                Raise(() => Settings);
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

        public bool HasSubjects { get; set; }
        public User() { }

        public void Save()
        {
            AppData.Instance.LiteConnection.DeleteAll<User>(false);
            AppData.Instance.LiteConnection.Insert(this, false);
            SchoolService.Save(this.School);
        }
        public Settings GetSettings()
        {
            Settings settings = AppData.Instance.LiteConnection
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
