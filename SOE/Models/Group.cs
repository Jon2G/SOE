using FirestoreLINQ;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
using SOE.API;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SOE.Models
{
    [FireStoreCollection("Groups")]
    public class Group : IComparable<Group>, IComparable
    {
        [Ignored]
        private static readonly Dictionary<string, Group> Cache = new Dictionary<string, Group>();
        [Ignored]
        private static readonly object _CacheLock = new object();
        [Id]
        public string DocumentId { get; set; }
        public string Name { get; set; }

        public static Group None => new Group() { Name = "XXX" };

        public static ICollectionReference Collection => FireBaseConnection.SchoolDocument.Collection<Group>();
        public Group()
        {

        }

        public Task<Group> Save()
        {
            DocumentId = this.GetDocumentId();
            return Collection.Document(DocumentId).SetAsync(this).ContinueWith(t => this);
        }
        public override string ToString() => Name;
        public string GetDocumentId() => $"group_{Name}";
        public static async Task<Group> GetCachedGroup(string groupId)
        {
            Group group = null;
            if (!Cache.TryGetValue(groupId, out group))
            {
                group = await Group.Get(groupId);
                lock (_CacheLock)
                {
                    if (!Cache.ContainsKey(groupId))
                        Cache.Add(groupId, group);
                }
            }
            return group;
        }
        private static Task<Group> Get(string groupId)
        {
            return Collection.Document(groupId).GetAsync().Get<Group>();
        }
        public int CompareTo(object obj)
        {
            if (obj is Group group)
            {
                group.CompareTo(this);
            }
            return -1;
        }
        public int CompareTo(Group other)
        {
            if (other is null)
                return -1;
            return other.GetDocumentId().CompareTo(this.DocumentId);
        }
    }
}
