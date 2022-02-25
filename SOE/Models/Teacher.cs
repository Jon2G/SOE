using Google.Cloud.Firestore;
using Kit.Model;
using SOE.Data;

using System.Threading.Tasks;

namespace SOE.Models
{
    [FirestoreData]
    public class Teacher : ModelBase
    {
        [FirestoreDocumentId]
        public string DocumentId { get; set; }
        [FirestoreProperty]
        public string Name { get; set; }

        public static Teacher Free => new Teacher() { Name = "LIBRE" };
        public async Task<Teacher> Save(School school)
        {
            this.DocumentId = this.GetDocumentId();
            var wr = await school.GetDocuent().Collection<Teacher>().Document(DocumentId).SetAsync(this);
            return this;
        }

        internal static async Task<Teacher> Get(string DocumentId)
        {
            var snapshot = await AppData.Instance.User.School.GetDocuent().Collection<Teacher>().Document(DocumentId).GetSnapshotAsync();
            return snapshot.ConvertTo<Teacher>();

        }
        public string GetDocumentId()
        {
            string subjectUniqueName = this.Name.ToLower();
            foreach (string s in new[] { ".", " ", "\n", "\r" })
            {
                subjectUniqueName = subjectUniqueName.Replace(s, string.Empty);
            }
            return subjectUniqueName;
        }
    }
}
