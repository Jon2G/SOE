using FirestoreLINQ;
using Kit;
using Kit.Model;
using Kit.Sql.Attributes;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
using Plugin.CloudFirestore.Converters;
using SOE.API;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SOE.Models
{
    [Preserve(AllMembers = true), FireStoreCollection("Classtimes")]
    public class ClassTime : ModelBase
    {
        [Id]
        public string IdDocument { get; set; }
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

        public string GroupId { get; set; }
        [DocumentConverter(typeof(EnumStringConverter))]
        public DayOfWeek Day { get; set; }
        [DocumentConverter(typeof(SOE.Converters.TimeSpanConverter))]
        public TimeSpan Begin
        {
            get;
            set;
        }
        [DocumentConverter(typeof(SOE.Converters.TimeSpanConverter))]
        public TimeSpan End
        {
            get;
            set;
        }
        public ClassTime()
        {

        }

        public ClassTime(string group, string subjectId, DayOfWeek Day, TimeSpan Begin, TimeSpan End)
        {
            this.GroupId = group;
            this.Day = Day;
            this.SubjectId = subjectId;
            this.Begin = Begin;
            this.End = End;
            IdDocument = this.GetDocumentId();
        }

        public async Task<Subject> GetSubject()
        {
            this.Subject = await Models.Subject.GetCachedSubject(SubjectId);
            this.Subject.GroupId = this.GroupId;
            return this.Subject;
        }
        public static ICollectionReference Collection =>
            FireBaseConnection.UserDocument.Collection<ClassTime>();
        public static async Task<IEnumerable<ClassTime>> GetAll()
        {
            IQuerySnapshot capitalQuerySnapshot = await Collection.GetAsync();
            return GetEnumerable(capitalQuerySnapshot);
        }
        public static IEnumerable<ClassTime> GetEnumerable(IQuerySnapshot capitalQuerySnapshot)
        {
            foreach (IDocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
            {
                yield return documentSnapshot.ToObject<ClassTime>();
            }
        }
        public static Task<IEnumerable<ClassTime>> IQuery(IQuery IQuery)
        {
            return IQuery.GetAsync().ContinueWith(t =>
           {
               return GetEnumerable(t.Result);
           });
        }
        public string GetDocumentId()
        {
            return String.Concat(Day.GetDayName(), GroupId, SubjectId);
        }

        public Task<ClassTime> Save()
        {
            return FireBaseConnection.UserDocument.Collection<ClassTime>()
                .Document(GetDocumentId()).SetAsync(this)
                .ContinueWith(t => this);
        }
    }
}
