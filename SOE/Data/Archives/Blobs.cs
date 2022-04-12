using FirestoreLINQ;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
using SOE.API;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SOE.Data.Archives
{
    [FireStoreCollection("Blobs")]
    public class Blobs
    {
        [Id]
        public string DocumentId { get; set; }

        public string Base64 { get; set; }

        public string ArchiveDocumentId { get; set; }

        public Blobs()
        {

        }
        public static ICollectionReference Collection =>
            FireBaseConnection.UserDocument.Collection<Blobs>();

        public static async IAsyncEnumerable<Blobs> IQuery(IQuery IQuery)
        {
            IQuerySnapshot capitalQuerySnapshot = await IQuery.GetAsync();
            foreach (IDocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
            {
                yield return documentSnapshot.ToObject<Blobs>();
            }
        }
        public static Task Save(Blobs blobs)
        {
            return Collection.AddAsync(blobs);
        }
    }
}
