using Kit.Sql.Attributes;
using SOE.Data;
using SOEWeb.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOE.Models.Data
{
    [Table("UserLocalData"), Preserve(AllMembers = true)]
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
        [PrimaryKey, NotNull]
        public string UserKey { get; set; }
        public string Boleta { get; set; }
        public string Password { get; set; }
        public string SchoolId { get; set; }

        public UserLocalData()
        {

        }

        public static UserLocalData Get()
        {
            return AppData.Instance.LiteConnection.Table<UserLocalData>().FirstOrDefault();
        }

        internal void Save()
        {
            AppData.Instance.LiteConnection.InsertOrReplace(this);
        }
    }
}
