using Kit.Sql.Attributes;
using SOEWeb.Shared.Interfaces;

namespace SOE.Models.Academic
{
    public class Career : IOffline
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsOffline { get; set; }
    }
}
