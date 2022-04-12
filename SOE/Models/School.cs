using FirestoreLINQ;

using Newtonsoft.Json;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
using SOE.API;
using System.Threading.Tasks;

namespace SOE.Models
{
    [FireStoreCollection("Schools")]
    public class School
    {
        [JsonProperty(nameof(DocumentId)), Id]
        public string DocumentId { get; set; }
        [JsonProperty(nameof(HomePage))]
        public string HomePage { get; set; }
        [JsonProperty(nameof(Name))]
        public string Name { get; set; }
        [JsonProperty(nameof(ImgPath))]
        public string ImgPath { get; set; }
        [JsonProperty(nameof(SchoolPage))]
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

        public static ICollectionReference Collection = FireBaseConnection.GetCollection<School>();
        public Task<School> Get(string id)
        {
            return Collection.Document(id).GetAsync().Get<School>();
        }
        public static Task<School> Get()
        {
            return FireBaseConnection.SchoolDocument.GetAsync().Get<School>();
        }
        public Task<School> Save()
        {
            return FireBaseConnection.SchoolDocument.SetAsync(this)
                .ContinueWith(t => this);

        }
    }
}
