using FirestoreLINQ;
using Kit.Model;
using Kit.Sql.Attributes;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
using SOE.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOE.Models
{
    [Preserve(AllMembers = true), FireStoreCollection("Subjects")]
    public class Subject : ModelBase, IComparable<Subject>, IEquatable<Subject>
    {
        [Ignored]
        private static readonly Dictionary<string, Subject> Cache = new Dictionary<string, Subject>();
        [Ignored]
        private static readonly object _CacheLock = new object();
        [Id]
        public string DocumentId { get; set; }

        public string Name { get; set; }
        private ThemeColor _ThemeColor;

        public ThemeColor ThemeColor
        {
            get => this._ThemeColor;
            set
            {
                this._ThemeColor = value;
                this.Raise(() => this.ThemeColor);
            }
        }
        public string Color
        {
            get => this.ThemeColor?.Light;
            set
            {
                if (this.ThemeColor is null)
                {
                    this.ThemeColor = new ThemeColor() { Light = value };
                }
                else
                {
                    this.ThemeColor.Light = value;
                }
            }
        }
        public string ColorDark
        {
            get => this.ThemeColor?.Dark;
            set
            {
                if (this.ThemeColor is null)
                {
                    this.ThemeColor = new ThemeColor() { Dark = value };
                }
                else
                {
                    this.ThemeColor.Dark = value;
                }
            }
        }
        public string GroupId
        {
            get;
            set;
        }

        public string TeacherId { get; set; }
        public Subject()
        {

        }
        public Subject(string teacherId, string Name, ThemeColor color, string group)
        {
            this.TeacherId = teacherId;
            this.Name = Name;
            this.ThemeColor = color;
            this.GroupId = group;
            this.DocumentId = this.GetDocumentId();
        }
        public Task<Teacher> GetTeacher()
        {
            return Teacher.Get(TeacherId);
        }
        public Task<Group> GetGroup()
        {
            return Group.GetCachedGroup(GroupId);
        }


        public static ICollectionReference Collection => FireBaseConnection.SchoolDocument.Collection<Subject>();

        public static Subject FreeTime => new Subject(Teacher.Free.GetDocumentId(), "Hora libre",
            new ThemeColor(Xamarin.Forms.Color.Gainsboro.ToHex()), Group.None.GetDocumentId());
        public static async Task<Subject> GetCachedSubject(string subjectId)
        {
            Subject subject = null;
            if (!Cache.TryGetValue(subjectId, out subject))
            {
                subject = await Subject.Get(subjectId);
                lock (_CacheLock)
                {
                    if (!Cache.ContainsKey(subjectId))
                        Cache.Add(subjectId, subject);
                }
            }

            return subject;
        }
        private static Task<Subject> Get(string documentId) =>
            Collection.Document(documentId).GetAsync().Get<Subject>();
        internal async Task<Subject> Save()
        {
            await Task.Yield();
            DocumentId = GetDocumentId();
            await Collection.Document(DocumentId).SetAsync(this);
            return this;
        }


        //public static ValueTask<List<Subject>> GetAll() => IQuery(Collection).ToListAsync();

        public static Task<Subject?> FindByName(string subjectName) =>
            IQuery(Collection.WhereEqualsTo(nameof(Name), subjectName)).FirstOrDefaultAsync().AsTask();


        public static async IAsyncEnumerable<Subject> IQuery(IQuery IQuery)
        {
            IQuerySnapshot capitalQuerySnapshot = await IQuery.GetAsync();
            foreach (IDocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
            {
                yield return documentSnapshot.ToObject<Subject>();
            }
        }
        public string GetDocumentId()
        {
            string subjectUniqueName = $"{this.Name.ToLower()}";
            foreach (string s in new[] { ".", " ", "\n", "\r" })
            {
                subjectUniqueName = subjectUniqueName.Replace(s, string.Empty);
            }
            return subjectUniqueName;
        }

        public Task<ClassTime> GetClassTime(DayOfWeek dateDayOfWeek)
        {
            return ClassTime.IQuery(ClassTime.Collection
                    .WhereEqualsTo(nameof(ClassTime.Day), dateDayOfWeek))
                .ContinueWith(t =>
                {
                    return t.Result.FirstOrDefault(x => x.Subject == this);
                });
        }

        public static bool operator !=(Subject original, Subject other)
        {
            return !(original == other);
        }
        public static bool operator ==(Subject original, Subject other)
        {
            if (other is null && original is null)
            {
                return true;
            }
            return original?.Equals(other) ?? false;
        }
        public override int GetHashCode()
        {
            return this.DocumentId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is not Subject other)
            {
                return false;
            }
            return other.Equals(other: other);
        }

        public int CompareTo(Subject other)
        {
            if (other is null) return -1;
            return other.GetDocumentId().CompareTo(this.GetDocumentId());
        }

        public bool Equals(Subject other)
        {
            return this.CompareTo(other) == 0;
        }

        public override string ToString()
        {
            return $"{Name}-{GroupId}";
        }
    }
}
