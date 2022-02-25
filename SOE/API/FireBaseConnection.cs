using FirestoreLINQ;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Kit;
using SOE.Data;

using SOE.Models;
using SOE.Models.Data;
using SOE.Secrets;
using SOEWeb.Shared;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SOE.API
{
    public class FireBaseConnection
    {
        public static FireBaseConnection Instance => _Instance.Value;
        private static readonly Lazy<FireBaseConnection> _Instance = new Lazy<FireBaseConnection>(() => new FireBaseConnection());
        public readonly FirestoreDb Database;
        public FirestoreClient Client => Database.Client;

        public DocumentReference UserDocument => Database.Collection<User>().Document(FireBaseConnection.UserPath);
        public IQueryable<School> Schools => Database.AsQuerable<School>();
        public static string UserPath => $"user_{AppData.Instance.User.Boleta}";

        public static IQueryable<T> GetQueryable<T>() where T : class
        {
            return Instance.Database.Collection<T>().AsQuerable<T>();
        }


        public static CollectionReference GetCollection<T>() where T : class
        {
            return Instance.Database.Collection<T>();
        }
        public static DocumentReference GetDocument<T>()
        {
            return Instance.Database.GetDocument<T>();
        }

        public static async Task<T> Add<T>(T obj) where T : IFireBaseKey
        {
            await Task.Yield();
            DocumentReference a = await FireBaseConnection.GetCollection<School>().AddAsync(obj);
            return obj;
        }
        public FireBaseConnection()
        {
            FirestoreDbBuilder builder = new FirestoreDbBuilder
            {
                ProjectId = DotNetEnviroment.FireStoreProjectId,
                JsonCredentials = DotNetEnviroment.FireBaseServiceAccountCredentials,
                WarningLogger = (l) => Log.Logger.Warning("FIREBASE:{0}", l)
            };
            Database = builder.Build();
        }
    }
}
