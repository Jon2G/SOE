using System;
using System.Collections.Generic;
using System.Text;
using Kit.Sql.Attributes;

namespace SchoolOrganizer.Models.Academic
{
    public class Teacher
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
