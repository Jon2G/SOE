using Kit.Sql.Attributes;

namespace SOE.Models.Academic
{
    public class Career
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
