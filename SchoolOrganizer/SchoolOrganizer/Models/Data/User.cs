using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Kit;
using Kit.Model;
using Kit.Sql.Attributes;
using SchoolOrganizer.Data;
using SchoolOrganizer.Data.Images;
using SchoolOrganizer.Saes;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SchoolOrganizer.Models.Data
{
    public class User : ModelBase
    {
        private string _Boleta;
        private string _Password;
        private bool _RemeberMe;
        private string _Name;
        private string _Career;
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
                    School = new School(value, String.Empty, String.Empty);
                }
                else
                {
                    School.HomePage = value;
                }

            }
        }


        public string Career
        {
            get => _Career;
            set
            {
                _Career = value;
                Raise(() => Career);
            }
        }
        internal static User Get()
        {
            User User = AppData.Instance.LiteConnection.Table<User>().FirstOrDefault();
            return User;
        }

        public string Name { get => _Name; set { _Name = value; Raise(() => Name); } }
        [Ignore]
        public bool IsLogedIn { get; set; }

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


        public Settings GetSettings()
        {
            var settings = AppData.Instance.LiteConnection
                .Table<Settings>()
                .FirstOrDefault(x => x.Boleta == this.Boleta);
            if (settings is null)
            {
                settings = new Settings()
                {
                    Boleta = this.Boleta
                };
                settings.Save();
            }

            return settings;
        }
    }
}
