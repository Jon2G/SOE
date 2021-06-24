using Kit.Sql.Attributes;

namespace SOE.Models.Academic
{
    public class Teacher
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
