using APIModels;
using DocumentFormat.OpenXml.Drawing.Charts;
using Kit.Model;
using Kit.Sql.Attributes;
using SOE.Data;
using SOE.Services;

namespace SOE.Models.Data
{
    public class User :UserBase
    {
        private string _Name;
        private string _Career;
        private string _Email;

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

        public bool HasSubjects { get; internal set; }

        public User() { }

        //internal static async Task<FileImageSource> SaveAvatar(FileResult result)
        //{
        //    FileInfo avatar = new FileInfo(Path.Combine(AppData.Instance.ImagesDirectory.FullName, $"{AppData.Instance.User.Boleta}.png"));
        //    await Keeper.Save(result.OpenReadAsync(), avatar);
        //    return GetAvatar();
        //}
        //internal static FileImageSource GetAvatar()
        //{
        //    DirectoryInfo directory = AppData.Instance.ImagesDirectory;
        //    if (!directory.Exists)
        //    {
        //        directory.Create();
        //        return null;
        //    }

        //    FileInfo avatar = new FileInfo(Path.Combine(directory.FullName, $"{AppData.Instance.User.Boleta}.png"));
        //    if (!avatar.Exists)
        //    {
        //        return null;
        //    }
        //    return (FileImageSource)FileImageSource.FromFile(avatar.FullName);
        //}

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
