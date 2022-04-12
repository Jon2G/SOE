using Kit;
using Kit.Model;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
using SOE.API;
using SOE.FireBase;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SOE.Models
{
    public class SchoolContact : ModelBase, IFireStoreObject
    {

        private string _Url;
        private string _Phone;
        private string _Name;
        private string _Correo;
        [Id]
        public string DocumentId { get; set; }

        private string _Departament;
        public string Departament
        {
            get => this._Departament;
            set
            {
                this._Departament = value;
                this.Raise(() => this.Departament);
            }
        }
        [Ignored]
        public string Initials => this.Name.ExtractInitialsFromName(2);
        public string Name
        {
            get => this._Name;
            set
            {
                this._Name = value;
                this.Raise(() => this.Name);
            }
        }
        public string Phone
        {
            get => this._Phone;
            set
            {
                this._Phone = value;
                this.Raise(() => this.Phone);
            }
        }
        public string Url
        {
            get => this._Url;
            set
            {
                this._Url = value;
                this.Raise(() => this.Url);
            }
        }
        public string Correo
        {
            get => this._Correo;
            set
            {
                this._Correo = value;
                this.Raise(() => this.Correo);
            }
        }

        public SchoolContact(string Departament, string Name, string Phone, string Url, string Correo)
        {
            this.Departament = Departament;
            this.Name = Name;
            this.Phone = Phone;
            this.Url = Url;
            this.Correo = Correo;
        }
        public SchoolContact()
        {

        }
        public string GetDocumentId()
        {
            return DocumentId;
        }
        public static ICollectionReference Collection => FireBaseConnection.SchoolDocument.Collection<SchoolContact>();

        public static Task<IEnumerable<SchoolContact>> IQuery(IQuery IQuery)
        {
            return IQuery.GetAsync().ContinueWith(t => GetEnumerable(t.Result));
        }

        public static IEnumerable<SchoolContact> GetEnumerable(IQuerySnapshot capitalQuerySnapshot)
        {
            foreach (IDocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
            {
                yield return documentSnapshot.ToObject<SchoolContact>();
            }
        }

        public static Task<IEnumerable<SchoolContact>> GetAll()
        {
            return IQuery(Collection.OrderBy(nameof(Departament)));
        }

        public Task<SchoolContact> Save()
        {
            return Collection.AddAsync(this).ContinueWith(t => this);
        }

        public Task Delete()
        {
            return Collection.Document(this.DocumentId).DeleteAsync();
        }
    }
}
