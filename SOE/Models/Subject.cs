using FirestoreLINQ;
using Google.Cloud.Firestore;
using Kit.Model;
using Kit.Sql.Attributes;
using SOE.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOE.Models
{
    [Preserve(AllMembers = true), FirestoreData, FireStoreCollection("Subjects")]
    public class Subject : ModelBase, IComparable<Subject>, IEquatable<Subject>
    {
        [FirestoreProperty]
        public string Name { get; set; }
        private ThemeColor _ThemeColor;
        [FirestoreProperty]
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
        [FirestoreProperty]
        public Group Group { get; set; }
        [FirestoreProperty]
        public Teacher Teacher { get; set; }

        [FirestoreDocumentId]
        public string DocumentId { get; set; }

        public static CollectionReference Collection => FireBaseConnection.Instance.UserDocument.Collection<Subject>();
        internal async Task<Subject> Save()
        {
            await Task.Yield();
            DocumentId = GetDocumentId();
            await Collection.Document(DocumentId).SetAsync(this);
            return this;
        }
        public Subject()
        {

        }
        public Subject(Teacher teacher, string Name, ThemeColor color, Group group)
        {
            this.Teacher = teacher;
            this.Name = Name;
            this.ThemeColor = color;
            this.Group = group;
            this.DocumentId = this.GetDocumentId();
        }

        public static ValueTask<List<Subject>> GetAll() => Query(Collection).ToListAsync();

        public static Task<Subject> FindByName(string subjectName) =>
            Query(Collection.WhereEqualTo(nameof(Name), subjectName)).FirstOrDefaultAsync().AsTask();
        public static Task<Subject> Get(string DocumentId) =>
            Query(Collection.WhereEqualTo(nameof(DocumentId), DocumentId)).FirstOrDefaultAsync().AsTask();
        public static async IAsyncEnumerable<Subject> Query(Query query)
        {
            QuerySnapshot capitalQuerySnapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
            {
                yield return documentSnapshot.ConvertTo<Subject>();
            }
        }
        private string GetDocumentId()
        {
            string subjectUniqueName = this.Name.ToLower();
            foreach (string s in new[] { ".", " ", "\n", "\r" })
            {
                subjectUniqueName = subjectUniqueName.Replace(s, string.Empty);
            }
            return subjectUniqueName;
        }

        public Task<ClassTime> GetClassTime(DayOfWeek dateDayOfWeek)
        {
            return ClassTime.Query(ClassTime.Collection.WhereEqualTo(nameof(ClassTime.Day), dateDayOfWeek))
                .Where(x => x.Subject == this)
                .FirstOrDefaultAsync().AsTask();
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
    }
}
