﻿using APIModels.Enums;
using Kit.Daemon.Sync;
using Kit.Sql.Attributes;

namespace APIModels
{

    [Table("Grades")]
    public class Grade : ISync
    {
        public const int Undefined = -1;
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public GradePartial Partial { get; set; }
        public string TextScore { get; set; }
        public int NumericScore { get; set; }

        public int SubjectId { get; set; }


        public Grade() { }
        public Grade(GradePartial Partial, string TextScore, int NumericScore, int SubjectId)
        {
            this.TextScore = TextScore;
            this.NumericScore = NumericScore;
            this.Partial = Partial;
            this.SubjectId = SubjectId;
        }

    }
}