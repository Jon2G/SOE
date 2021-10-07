using Kit.Sql.Attributes;
using SOEWeb.Shared.Interfaces;

namespace SOEWeb.Shared
{
    public class Teacher : IOffline
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsOffline { get; set; }
    }
}
