using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SOE.Models
{
    public class Link
    {
        [Id]
        public string DocumentId { get; set; }
        [MapTo(nameof(Name))]
        public string Name { get; set; }
        [MapTo(nameof(Url))]
        public string Url { get; set; }

        [Ignored]
        public string ImageUrl
        {
            get
            {
                string google_service = $"https://www.google.com/s2/favicons?sz=64&domain_url={this.Url}";
                return google_service;
            }
        }

        public static ICollectionReference Collection(Subject subject)
        {
            return Subject.Collection.Document(subject.DocumentId).Collection<Link>();
        }
        public Link()
        {

        }
        public Link(string Name, string Url)
        {
            this.Name = Name;
            this.Url = Url;
        }

        public string GetDocumentId()
        {
            return HttpUtility.UrlEncode(this.Url, Encoding.UTF7);
        }

        public static Task<IEnumerable<Link>> GetLinks(Subject subject)
        {
            return Collection(subject).GetAsync().ContinueWith(t =>
             {
                 return GetEnumerable(t.Result);
             });
        }
        public Task<Link> Save(Subject subject)
        {
            this.DocumentId = this.GetDocumentId();
            return Collection(subject).AddAsync(this).ContinueWith(t => this);
        }

        public Task Delete(Subject subject)
        {
            return Collection(subject).Document(this.DocumentId).DeleteAsync();
        }

        public static IEnumerable<Link> GetEnumerable(IQuerySnapshot capitalQuerySnapshot)
        {
            foreach (IDocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
            {
                yield return documentSnapshot.ToObject<Link>();
            }
        }
        public static Task<IEnumerable<Link>> IQuery(IQuery IQuery)
        {
            return IQuery.GetAsync().ContinueWith(t =>
            {
                return GetEnumerable(t.Result);
            });
        }
    }
}
