using FirestoreLINQ;
using Google.Cloud.Firestore;
using SOE.API;
using System.Threading.Tasks;

namespace SOE.Models
{
    [FirestoreData, FireStoreCollection("Groups")]
    public class Group
    {
        [FirestoreProperty]
        public string Name { get; set; }
        [FirestoreDocumentId]
        public string DocumentId { get; set; }

        public static Group None => new Group() { Name = "XXX" };

        public Group()
        {

        }

        public async Task<Group> Save()
        {
            DocumentId = Name;
            await FireBaseConnection.GetCollection<Group>().Document(DocumentId).SetAsync(this);
            return this;
        }
        public override string ToString() => Name;
    }
}
