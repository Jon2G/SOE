using FirestoreLINQ;
using Google.Cloud.Firestore;
using Newtonsoft.Json;
using SOE.API;
using SOEWeb.Shared;
using System.Threading.Tasks;

namespace SOE.Models
{
    [FirestoreData, FireStoreCollection("Schools")]
    public class School
    {
        [JsonProperty(nameof(DocumentId)), FirestoreProperty]
        public string DocumentId { get; set; }
        [JsonProperty(nameof(HomePage)), FirestoreProperty]
        public string HomePage { get; set; }
        [JsonProperty(nameof(Name)), FirestoreProperty]
        public string Name { get; set; }
        [JsonProperty(nameof(ImgPath)), FirestoreProperty]
        public string ImgPath { get; set; }
        [JsonProperty(nameof(SchoolPage)), FirestoreProperty]
        public string SchoolPage { get; set; }
        public School()
        {

        }
        public School(string HomePage, string Name, string ImgPath, string SchoolPage)
        {
            this.HomePage = HomePage;
            this.Name = Name;
            this.ImgPath = ImgPath;
            this.SchoolPage = SchoolPage;
        }

        public DocumentReference GetDocuent()
        {
            return FireBaseConnection.GetCollection<School>().Document(this.DocumentId);
        }
    }
}
