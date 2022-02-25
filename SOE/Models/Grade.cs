using FirestoreLINQ;
using Google.Cloud.Firestore;
using Kit.Model;
using SOE.API;
using SOE.Enums;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SOE.Models
{

    [FireStoreCollection("Grades"), FirestoreData]
    public class Grade : ModelBase
    {
        public const int Undefined = -1;
        [FirestoreDocumentId]
        public string DocumentId { get; set; }
        [FirestoreProperty]
        public GradePartial Partial { get; set; }
        [FirestoreProperty]
        public string TextScore { get; set; }
        [FirestoreProperty]
        public int NumericScore { get; set; }
        [FirestoreProperty]
        public Subject Subject { get; set; }

        public Grade() { }
        public Grade(GradePartial Partial, string TextScore, int NumericScore, Subject subject)
        {
            this.TextScore = TextScore;
            this.NumericScore = NumericScore;
            this.Partial = Partial;
            this.Subject = subject;
            this.DocumentId = this.GetDocumentId();
        }
        private string GetDocumentId()
        {
            return String.Concat(Subject.DocumentId, Partial);
        }
        public static CollectionReference Collection =>
            FireBaseConnection.Instance.UserDocument.Collection<Grade>();

        public static async IAsyncEnumerable<Grade> Query(Query query)
        {
            QuerySnapshot capitalQuerySnapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
            {
                yield return documentSnapshot.ConvertTo<Grade>();
            }
        }
        public async Task<Grade> Save()
        {
            await Task.Yield();
            DocumentId = this.GetDocumentId();
            await FireBaseConnection.Instance.UserDocument.Collection<Grade>()
                .Document(this.DocumentId).SetAsync(this);
            return this;
        }
    }
}
