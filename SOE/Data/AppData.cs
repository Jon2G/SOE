using System;
using System.IO;
using APIModels;
using Kit.Forms.Services.Interfaces;
using Kit.Model;
using Kit.Sql.Sqlite;
using SOE.Data.Images;
using SOE.Models;
using SOE.Models.Academic;
using SOE.Models.Data;
using SOE.Models.Scheduler;
using SOE.Models.TaskFirst;
using SOE.Notifications;
using SOE.Saes;
using Xamarin.Forms;
using Device = Xamarin.Forms.Device;

namespace SOE.Data
{
    public class AppData : ModelBase
    {
        public const string Version = "1.0.0 BETA";
        public static bool IsInitialized => _Instance is not null;
        private static AppData _Instance;

        public static AppData Instance
        {
            get
            {
                if (IsInitialized)
                    return _Instance;
                return Init();
            }
        }

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


        public static AppData Init()
        {
            AppData._Instance = new AppData
            {
                User = new User(),
                LiteConnection = new SQLiteConnection(LiteDbPath, 100)
            };
            return Instance;
        }

        public static void CreateDatabase()
        {
            AppData.Instance.LiteConnection.CheckTables(
                typeof(Teacher), typeof(Subject), typeof(User), 
                typeof(Career), typeof(ClassTime), typeof(Grade), 
                typeof(Credits), typeof(ToDo), typeof(Settings), 
                typeof(NotificationsHistory), typeof(Document), 
                typeof(DocumentPart), typeof(Archive),
                typeof(Keeper),typeof(School),typeof(Reminder));
        }
  
    }
}
