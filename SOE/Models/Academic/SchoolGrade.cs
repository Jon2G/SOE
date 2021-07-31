using System.Collections.Generic;
using SOEWeb.Shared;
using SOEWeb.Shared.Enums;
using Kit.Model;
using SOE.Data;

namespace SOE.Models.Academic
{
    public class SchoolGrade : ModelBase
    {
        public Subject Subject { get; set; }
        public List<Grade> Grades { get; set; }

        public SchoolGrade(Subject Subject)
        {
            this.Subject = Subject;
            this.Grades = AppData.Instance.LiteConnection.Table<Grade>().Where(x => x.SubjectId == this.Subject.Id)
                .OrderBy(x => (int)x.Partial).ToList();
            int i = this.Grades.Count;
            while (this.Grades.Count < 5)
            {
                this.Grades.Add(new Grade((GradePartial)i, "-",Grade.Undefined, this.Subject.Id));
            }
        }
    }
}
