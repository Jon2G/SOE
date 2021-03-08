using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Kit;
using Kit.Sql.Helpers;
using Kit.Sql.Sqlite;
using SchoolOrganizer.Models;
using SchoolOrganizer.Models.Academic;
using SchoolOrganizer.Models.Data;
using SchoolOrganizer.Models.Scheduler;

namespace SchoolOrganizer.Data
{
    public class AppData
    {
        public static AppData Instance { get; private set; }

        public User User
        {
            get;
            set;
        }
        public SQLiteConnection LiteConnection { get; private set; }
        private AppData()
        {

        }

        public static void Init()
        {
            AppData.Instance = new AppData();
            AppData.Instance.User = new User();
            AppData.Instance.LiteConnection =
                new SQLiteConnection(
                    new FileInfo(
                        Path.Combine(Tools.Instance.LibraryPath, "SchoolOrganizer.db")), 104);
            AppData.Instance.LiteConnection.CheckTables(typeof(Teacher), typeof(Subject),typeof(User));
        }
    }
}
