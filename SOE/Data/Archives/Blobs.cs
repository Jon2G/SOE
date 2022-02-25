using FirestoreLINQ;
using Google.Cloud.Firestore;
using SOE.API;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SOE.Data.Archives
{
    [FirestoreData, FireStoreCollection("Blobs")]
    public class Blobs
    {
        [FirestoreDocumentId]
        public string DocumentId { get; set; }
        [FirestoreProperty]
        public string Base64 { get; set; }
        [FirestoreProperty]
        public string ArchiveDocumentId { get; set; }

        public Blobs()
        {

        }
        public static CollectionReference Collection =>
            FireBaseConnection.Instance.UserDocument.Collection<Blobs>();

        public static async IAsyncEnumerable<Blobs> Query(Query query)
        {
            QuerySnapshot capitalQuerySnapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
            {
                yield return documentSnapshot.ConvertTo<Blobs>();
            }
        }
        public static Task Save(Blobs blobs)
        {
            return Collection.AddAsync(blobs);
        }
    }
}
