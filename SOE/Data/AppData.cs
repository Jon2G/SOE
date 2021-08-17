using System;
using System.IO;
using SOEWeb.Shared;
using Kit.Model;
using Kit.Sql.Sqlite;
using SOE.Data.Images;
using SOE.Models;
using SOE.Models.Academic;
using SOE.Models.Data;
using SOE.Models.TaskFirst;
using SOE.Notifications;
using SOEWeb.Shared.Secrets;
using Xamarin.Essentials;

namespace SOE.Data
{
    public class AppData : ModelBase
    {
        public static string Version = $"{VersionTracking.CurrentVersion} BETA";
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

        public Saes.SAES SAES
        {
            get;
            set;
        }
        public SQLiteConnection LiteConnection { get; private set; }
        private AppData()
        {
            DotNetEnviroment.Load();
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

        public void ClearData(params Type[] Tables)
        {
            foreach (Type type in Tables)
            {
                var table = AppData.Instance.LiteConnection.Table(type);
                AppData.Instance.LiteConnection.Execute($"DROP TABLE IF EXISTS {table.Table.TableName}");
                AppData.Instance.LiteConnection.CreateTable(table.Table);
            }

            AppData.Instance.User.HasSubjects = false;
            AppData.Instance.User.Save();
        }
        public static void CreateDatabase()
        {
            AppData.Instance.LiteConnection.CheckTables(
                typeof(Teacher), typeof(Subject), typeof(User),
                typeof(Career), typeof(ClassTime), typeof(Grade),
                typeof(Credits), typeof(ToDo), typeof(Settings),
                typeof(NotificationsHistory), typeof(Document),
                typeof(DocumentPart), typeof(Archive),
                typeof(Keeper), typeof(School), typeof(Reminder),
                typeof(InscriptionDate));
        }

    }
}
