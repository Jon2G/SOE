using LiteDB;
using SOE.Data;
using System;
using System.Linq;
using Xamarin.Forms.Internals;

namespace SOE.Models.Data
{
    [Preserve(AllMembers = true)]
    public class UserLocalData
    {
        public static readonly Lazy<UserLocalData> _Instance = new Lazy<UserLocalData>(Get);
        public static UserLocalData Instance
        {
            get
            {
                if (_Instance.IsValueCreated && _Instance.Value is null)
                {
                    return Get();
                }
                return _Instance.Value;
            }
        }
        [BsonId(true)]
        public ObjectId Id { get; set; }
        public string UserKey { get; set; }
        public string Boleta { get; set; }
        public string Password { get; set; }
        public string SchoolId { get; set; }

        public UserLocalData()
        {
        }

        public static UserLocalData Get()
        {
            return AppData.Instance.LiteDatabase.GetCollection<UserLocalData>().FindAll()?.FirstOrDefault()
                   ?? new UserLocalData();
        }

        public void Save()
        {
            AppData.Instance.LiteDatabase.GetCollection<UserLocalData>().Upsert(this);
        }
    }
}
