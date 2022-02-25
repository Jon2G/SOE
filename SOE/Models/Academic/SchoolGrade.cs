using Kit.Model;
using SOE.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOE.Models.Academic
{
    public class SchoolGrade : ModelBase
    {
        public Subject Subject { get; private set; }
        public List<Grade> Grades { get; private set; }

        private SchoolGrade(Subject subject, List<Grade> grades)
        {
            this.Subject = subject;
            this.Grades = grades;
        }

        public static Task<SchoolGrade> FromSubject(Subject subject)
        {
            return Grade.Query(Grade.Collection.WhereEqualTo(nameof(Grade.Subject), subject))
                  .ToListAsync()
                  .AsTask()
                  .ContinueWith(t => new SchoolGrade(subject, t.Result))
                  .ContinueWith(t =>
                  {
                      var grade = t.Result;
                      int i = grade.Grades.Count;
                      while (grade.Grades.Count < 5)
                      {
                          grade.Grades.Add(new Grade((GradePartial)i, "-", Grade.Undefined, subject));
                      }
                      return grade;
                  });
        }
    }
}
