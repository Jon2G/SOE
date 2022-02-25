using FirestoreLINQ;
using Google.Cloud.Firestore;
using Kit.Sql.Attributes;
using SOE.API;
using System.Threading.Tasks;

namespace SOE.Models.Academic
{
    [FireStoreCollection("Credits"), FirestoreData, Preserve(AllMembers = true)]
    public class Credits
    {
        [FirestoreDocumentId]
        public string DocumentId { get; set; }
        [FirestoreProperty]
        public double CurrentCredits { get; set; }
        [FirestoreProperty]
        public double TotalCredits { get; set; }
        [FirestoreProperty]
        public double Percentage { get; set; }
        public Credits() { }
        public Task Save()
        {
            return FireBaseConnection.Instance.UserDocument.Collection<Credits>()
                .Document("Credits").SetAsync(this);
        }

        public static async Task<Credits> Get()
        {
            var snap = await FireBaseConnection.Instance.UserDocument.Collection<Credits>()
                .Document("Credits").GetSnapshotAsync();
            return snap.ConvertTo<Credits>();
        }
    }
}
