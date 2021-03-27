﻿using System;
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
        public static bool IsInitialized => Instance is not null;
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

        private static FileInfo LiteDbPath => 
            new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "SchoolOrganizer.db"));
     

        public static void Init()
        {
            AppData.Instance = new AppData
            {
                User = new User(),
                LiteConnection = new SQLiteConnection(LiteDbPath, 116)
            };
            AppData.Instance.LiteConnection.CheckTables(typeof(Teacher), typeof(Subject), typeof(User), typeof(ClassTime));
        }
    }
}
