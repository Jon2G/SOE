using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Kit;
using Kit.Model;
using Kit.Sql.Helpers;
using Kit.Sql.Sqlite;
using SchoolOrganizer.Models;
using SchoolOrganizer.Models.Academic;
using SchoolOrganizer.Models.Data;
using SchoolOrganizer.Models.Scheduler;
using SchoolOrganizer.Models.TaskFirst;
using SchoolOrganizer.Saes;

namespace SchoolOrganizer.Data
{
    public class AppData : ModelBase
    {
        public static bool IsInitialized => Instance is not null;
        public static AppData Instance { get; private set; }

        private User _User;
        public User User
        {
            get => _User;
            set
            {
                _User = value;
                Raise(() => User);
            }
        }

        public Saes.SAES SAES
        {
            get;
            set;
        }
        public SQLiteConnection LiteConnection { get; private set; }
        private AppData()
        {

        }

        private static FileInfo LiteDbPath =>
            new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "SchoolOrganizer.db"));

        public DirectoryInfo ImagesDirectory =>
            new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                "Images"));

        public static void Init()
        {
            AppData.Instance = new AppData
            {
                User = new User(),
                LiteConnection = new SQLiteConnection(LiteDbPath, 116)
            };
            AppData.Instance.LiteConnection.CheckTables(typeof(Teacher), typeof(Subject), typeof(User), typeof(ClassTime), typeof(Grade), typeof(Credits),typeof(ToDo));
        }
    }
}
