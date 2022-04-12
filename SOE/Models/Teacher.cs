
using Kit.Model;
using Plugin.CloudFirestore.Attributes;
using SOE.API;

using System.Threading.Tasks;

namespace SOE.Models
{

    public class Teacher : ModelBase
    {
        [Id]
        public string DocumentId { get; set; }

        public string Name { get; set; }

        public static Teacher Free => new Teacher() { Name = "LIBRE" };
        public async Task<Teacher> Save(School school)
        {
            this.DocumentId = this.GetDocumentId();
            await FireBaseConnection.SchoolDocument.Collection<Teacher>().Document(DocumentId).SetAsync(this);
            return this;
        }

        internal static async Task<Teacher> Get(string DocumentId)
        {
            var snapshot = await FireBaseConnection.SchoolDocument.Collection<Teacher>().Document(DocumentId).GetAsync();
            return snapshot.ToObject<Teacher>();

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
