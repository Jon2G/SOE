using FirestoreLINQ;
using Google.Cloud.Firestore;
using Kit.Sql.Attributes;
using SOE.API;
using System.Threading.Tasks;

namespace SOE.Models.Data
{
    [FirestoreData, FireStoreCollection("InscriptionDate")]
    public class InscriptionDate
    {
        [FirestoreDocumentId, Preserve(AllMembers = true)]
        public string DocumentId { get; set; }
        [FirestoreProperty]
        public string Date { get; set; }

        public InscriptionDate()
        {

        }
        public InscriptionDate(string Date)
        {
            this.Date = Date;
        }

        public Task Save()
        {
            return FireBaseConnection.Instance.UserDocument.Collection<InscriptionDate>()
                    .Document("InscriptionDate").SetAsync(this);
        }

        public static async Task<InscriptionDate> Get()
        {
            var snap = await FireBaseConnection.Instance.UserDocument.Collection<InscriptionDate>()
                 .Document("InscriptionDate").GetSnapshotAsync();
            return snap.ConvertTo<InscriptionDate>();
        }
    }
}
