using FirestoreLINQ;

using Kit.Model;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
using SOE.API;
using SOE.Enums;
using SOE.FireBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOE.Models
{

    [FireStoreCollection("Grades")]
    public class Grade : ModelBase, IFireStoreObject
    {
        public const int Undefined = -1;
        [Id]
        public string DocumentId { get; set; }

        public GradePartial Partial { get; set; }

        public string TextScore { get; set; }
        public int NumericScore { get; set; }

        public string SubjectId { get; set; }
        [Ignored]
        private Subject _Subject;
        [Ignored]
        public Subject Subject
        {
            get
            {
                return this._Subject;
            }
            set
            {
                this._Subject = value;
            }
        }
        public async Task<Subject> GetSubject()
        {
            this.Subject = await Models.Subject.GetCachedSubject(SubjectId);
            return this.Subject;
        }

        public Grade() { }
        public Grade(GradePartial Partial, string TextScore, int NumericScore, string subjectId)
        {
            this.TextScore = TextScore;
            this.NumericScore = NumericScore;
            this.Partial = Partial;
            this.SubjectId = subjectId;
            this.DocumentId = this.GetDocumentId();
        }
        public string GetDocumentId()
        {
            return String.Concat(SubjectId, Partial);
        }

        public static Task<Grade[]> GetAll()
        {
            return Collection.OrderBy(nameof(SubjectId)).OrderBy(nameof(Partial)).GetAsync().GetEnumerable().ToArrayAsync().AsTask();
        }
        public static ICollectionReference Collection =>
            FireBaseConnection.UserDocument.Collection<Grade>();

        public static Task<IEnumerable<Grade>> IQuery(IQuery IQuery)
        {
            return IQuery.GetAsync().ContinueWith(t => GetEnumerable(t.Result));
        }

        public static IEnumerable<Grade> GetEnumerable(IQuerySnapshot capitalQuerySnapshot)
        {
            foreach (IDocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
            {
                yield return documentSnapshot.ToObject<Grade>();
            }
        }
        public Task<Grade> Save()
        {
            DocumentId = this.GetDocumentId();
            return FireBaseConnection.UserDocument.Collection<Grade>()
                .Document(this.DocumentId).SetAsync(this)
                .ContinueWith(t => this);
        }
    }
}
