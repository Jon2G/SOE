using System;
using System.Collections.Generic;
using System.Text;
using Kit.Daemon.Sync;
using Kit.Sql.Attributes;

namespace SchoolOrganizer.Models.Academic
{
    public enum Partial
    {
        First = 0, Second = 1, Third = 2, Extraordinary = 4, Final = 5
    }
    [Table("Grades")]
    public class Grade : ISync
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public Partial Partial { get; set; }
        public string Score { get; set; }
        public int SubjectId { get; set; }

        public Grade() { }
        public Grade(Partial Partial, string Score, int SubjectId)
        {
            this.Score = Score;
            this.Partial = Partial;
            this.SubjectId = SubjectId;
        }

    }
}
