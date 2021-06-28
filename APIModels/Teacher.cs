using Kit.Sql.Attributes;
using Kit.Sql.Readers;
using System;

namespace APIModels
{
    public class Teacher
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }


    }
}
