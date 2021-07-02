using System;
using System.IO;
using APIModels;
using Kit.Model;
using Kit.Sql.Sqlite;
using SOE.Data.Images;
using SOE.Models.Academic;
using SOE.Models.Data;
using SOE.Models.Scheduler;
using SOE.Models.TaskFirst;
using SOE.Notifications;
using SOE.Saes;

namespace SOE.Data
{
    public class AppData : ModelBase
    {
        public const string Version = "1.0.0 BETA";
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

        public SAES SAES
        {
            get;
            set;
        }
        public SQLiteConnection LiteConnection { get; private set; }
        private AppData()
        {

        }

        private static FileInfo LiteDbPath => new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "SOE.db"));


        public static void Init()
        {
            AppData.Instance = new AppData
            {
                User = new User(),
                LiteConnection = new SQLiteConnection(LiteDbPath, 100)
            };

        }

        public static void CreateDatabase()
        {
            AppData.Instance.LiteConnection.CheckTables(
                typeof(Teacher), typeof(Subject), typeof(User), 
                typeof(Career), typeof(ClassTime), typeof(Grade), 
                typeof(Credits), typeof(ToDo), typeof(Settings), 
                typeof(NotificationsHistory), typeof(Document), 
                typeof(DocumentPart), typeof(Archive),
                typeof(Keeper),typeof(School));
        }
    }
}
