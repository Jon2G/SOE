using Kit.Model;
using System.Collections.Generic;

namespace SOE.Models.Academic
{
    public class SchoolGrade : ModelBase
    {
        public Subject Subject { get; private set; }
        public List<Grade> Grades { get; private set; }
        public Group Group { get; set; }
        public SchoolGrade(Subject subject, List<Grade> grades, Group group)
        {
            this.Subject = subject;
            this.Group = group;
            this.Grades = grades;
        }
    }
}
