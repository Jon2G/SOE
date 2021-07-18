using Kit.Sql.Attributes;

namespace SOEWeb.Shared
{
    public class Teacher
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }


    }
}
