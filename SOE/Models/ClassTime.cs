using FirestoreLINQ;
using Google.Cloud.Firestore;
using Kit;
using Kit.Model;
using Kit.Sql.Attributes;
using SOE.API;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOE.Models
{
    [Preserve(AllMembers = true), FirestoreData, FireStoreCollection("Classtimes")]
    public class ClassTime : ModelBase
    {

        [FirestoreProperty]
        public Subject Subject { get; set; }
        [FirestoreDocumentId]
        public string IdDocument { get; set; }
        [FirestoreProperty]
        public Group Group { get; set; }
        [FirestoreProperty]
        public DayOfWeek Day { get; set; }

        [FirestoreProperty]
        public int BeginTimestamp
        {
            get => _Begin.ToFirestoreTime();
            set => this._Begin = FireStoreExtensions.ToFirestoreTime(value);
        }

        private TimeSpan _Begin;
        public TimeSpan Begin
        {
            get => _Begin;
            set => this._Begin = value;
        }
        [FirestoreProperty]
        public int EndTimestamp
        {
            get => _End.ToFirestoreTime();
            set => this._End = FireStoreExtensions.ToFirestoreTime(value);
        }
        private TimeSpan _End;
        public TimeSpan End
        {
            get => this._End;
            set => _End = value;
        }
        public ClassTime()
        {

        }

        public ClassTime(Group group, Subject subject, DayOfWeek Day, TimeSpan Begin, TimeSpan End)
        {
            this.Group = group;
            this.Day = Day;
            this.Subject = subject;
            this.Begin = Begin;
            this.End = End;
            IdDocument = this.GetDocumentId();
        }
        public static CollectionReference Collection =>
            FireBaseConnection.Instance.UserDocument.Collection<ClassTime>();

        public static async IAsyncEnumerable<ClassTime> Query(Query query)
        {
            QuerySnapshot capitalQuerySnapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
            {
                yield return documentSnapshot.ConvertTo<ClassTime>();
            }
        }
        public string GetDocumentId()
        {
            return String.Concat(Day.GetDayName(), Group.DocumentId, Subject.DocumentId);
        }

        public async Task<ClassTime> Save()
        {
            await Task.Yield();
            IdDocument = this.GetDocumentId();
            await FireBaseConnection.Instance.UserDocument.Collection<ClassTime>()
                    .Document(this.IdDocument).SetAsync(this);
            return this;
        }
        public static IQueryable<ClassTime> GetAsQuerable()
        {
            return FireBaseConnection.Instance.UserDocument.Collection<ClassTime>()
                .AsQuerable<ClassTime>();
        }
    }
}
