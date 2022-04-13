using Kit;
using Kit.Model;
using Kit.Sql.Sqlite;
using SOE.Models.Data;
using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms.Internals;

namespace SOE.Data
{
    [Preserve(AllMembers = true)]
    public class AppData : ModelBase
    {
        public static string Version = $"{VersionTracking.CurrentVersion}";

        private static readonly Lazy<AppData> _Instance = new Lazy<AppData>(() =>
        {
            FileInfo liteDbPath = new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "SOE.db"));
            var liteConnection = new SQLiteConnection(liteDbPath, 100);
            liteConnection.CreateTable<UserLocalData>();
            return new AppData
            {
                User = new User(),
                LiteConnection = liteConnection
            };
        });

        public static AppData Instance => _Instance.Value;

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

        public Saes.Saes SAES
        {
            get;
            set;
        }
        public SQLiteConnection LiteConnection { get; private set; }

        public static bool HasConnectivity =>
            Connectivity.NetworkAccess == NetworkAccess.ConstrainedInternet ||
            Connectivity.NetworkAccess == NetworkAccess.Internet;

        public static async Task<bool> HasInternetAccess()
        {
            PingReply reply = await new Ping().PingOrTimeout("8.8.8.8"); //Google Public DNS IP address
            return reply?.Status == IPStatus.Success;
        }

        public static Task<bool> EnsureHasInternetAccess()
        {
            return AppData.HasConnectivity ? AppData.HasInternetAccess() : Task.FromResult(false);
        }

        private AppData()
        {

        }

        //public void ClearData(params Type[] Tables)
        //{
        //    foreach (Type type in Tables)
        //    {
        //        var table = AppData.Instance.LiteConnection.Table(type);
        //        AppData.Instance.LiteConnection.Execute($"DROP TABLE IF EXISTS {table.Table.TableName}");
        //        AppData.Instance.LiteConnection.CreateTable(table.Table);
        //    }

        //    AppData.Instance.User.HasSubjects = false;
        //  await  AppData.Instance.User.Save();
        //}
    }
}
