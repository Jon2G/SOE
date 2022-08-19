using Plugin.CloudFirestore;
using SOE.Models;
using SOE.Models.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOE.API
{
    public static class FireBaseConnection
    {
        public static IFirestore Database => CrossCloudFirestore.Current.Instance;
        public static string UserPath => GetUserPath(UserLocalData.Instance.Boleta);
        public static IDocumentReference UserDocument => GetUserDocument(FireBaseConnection.UserPath);
        public static IDocumentReference SchoolDocument => School.Collection.Document(UserLocalData.Instance.SchoolId);

        public static ICollectionReference GetCollection<T>() where T : class
        {
            return Database.Collection<T>();
        }
        public static IDocumentReference GetDocument<T>()
        {
            return Database.GetDocument<T>();
        }

        public static string GetUserPath(string boleta) => $"user_{boleta}";
        public static IDocumentReference GetUserDocument(string userPath) => SchoolDocument.Collection<User>().Document(userPath);

        public static async IAsyncEnumerable<Grade> GetEnumerable(this Task<IQuerySnapshot> task)
        {
            IQuerySnapshot? capitalQuerySnapshot = await task;
            foreach (IDocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
            {
                yield return documentSnapshot.ToObject<Grade>();
            }
        }
        public static async Task<T> GetFistOrDefault<T>(this Task<IQuerySnapshot> task) where T : class
        {
            IQuerySnapshot? capitalQuerySnapshot = await task;
            return capitalQuerySnapshot.GetFistOrDefault<T>();
        }
        public static T GetFistOrDefault<T>(this IQuerySnapshot capitalQuerySnapshot) where T : class
        {
            return capitalQuerySnapshot.Documents.FirstOrDefault()?.ToObject<T>();
        }
        public static IEnumerable<T> GetEnumerable<T>(this IQuerySnapshot capitalQuerySnapshot) where T : class
        {
            foreach (IDocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
            {
                yield return documentSnapshot.ToObject<T>();
            }
        }

        public static async Task<T> Get<T>(this Task<IDocumentSnapshot> task) where T : class
        {
            IDocumentSnapshot documentSnapshot = await task;
            return documentSnapshot.ToObject<T>();
        }
        static FireBaseConnection()
        {
            CrossCloudFirestore.Current.Instance.FirestoreSettings = new FirestoreSettings
            {
                CacheSizeBytes = FirestoreSettings.CacheSizeUnlimited,
                IsPersistenceEnabled = true
            };
        }
    }
}
