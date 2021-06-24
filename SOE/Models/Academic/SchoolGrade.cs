using System.Collections.Generic;
using Kit.Model;
using SOE.Data;
using SOE.Models.Scheduler;

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
                this.Grades.Add(new Grade((Partial)i, "-", this.Subject.Id));
            }
        }
    }
}
