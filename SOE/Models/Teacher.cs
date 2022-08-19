
using Kit.Model;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
using SOE.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOE.Models
{

    public class Teacher : ModelBase, IComparable<Teacher>, IComparable
    {
        [Id]
        public string DocumentId { get; set; }

        private string? _name;
        public string Name { get => this._name ?? string.Empty; set => RaiseIfChanged(ref this._name, value); }

        public static Teacher Free => new Teacher() { Name = "LIBRE" };
        public static ICollectionReference Collection => FireBaseConnection.SchoolDocument.Collection<Teacher>();

        public Teacher()
        {

        }
        public async Task<Teacher> Save()
        {
            this.DocumentId = this.GetDocumentId();
            await Collection.Document(DocumentId).SetAsync(this);
            return this;
        }

        internal static async Task<IEnumerable<Teacher>> Query(IQuery query)
        {
            IQuerySnapshot? snapshot = await query.GetAsync();
            return snapshot?.ToObjects<Teacher>() ?? Array.Empty<Teacher>();
        }

        internal static async Task<IEnumerable<Teacher>> GetAll()
        {
            IQuerySnapshot snapshot = await Collection.GetAsync();
            var teachers = snapshot.ToObjects<Teacher>().ToList() ?? new List<Teacher>();
            teachers.Add(Teacher.Free);
            return teachers;
        }
        internal static async Task<Teacher?> Get(string DocumentId)
        {
            IDocumentSnapshot? snapshot = await Collection.Document(DocumentId).GetAsync();
            return snapshot?.ToObject<Teacher>();

        }
        public string GetDocumentId()
        {
            string subjectUniqueName = this.Name?.ToLower()?.Trim() ?? string.Empty;
            foreach (string s in new[] { ".", " ", "\n", "\r" })
            {
                subjectUniqueName = subjectUniqueName.Replace(s, string.Empty);
            }
            return subjectUniqueName;
        }

        public int CompareTo(Teacher other)
        {
            return other?.GetDocumentId()?.CompareTo(this.GetDocumentId()) ?? -1;
        }

        public int CompareTo(object obj)
        {
            if (obj is Teacher teacher)
            {
                return teacher.CompareTo(this);
            }
            return -1;
        }
    }
}
