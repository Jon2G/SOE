using Kit.Model;
using Kit.Sql.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOEWeb.Shared
{
    public class SubjectComment : List<SubjectComment>
    {
        [PrimaryKey, AutoIncrement, NotNull]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        [Ignore]
        public string UserName { get; set; }
        public string TeacherName { get; set; }
        public string GroupName { get; set; }
        [MaxLength(140)]
        public string Text { get; set; }
        public bool HasSubComments { get; set; }
        public bool OpenSubComments { get; set; }
        public bool CommentBox { get; set; }
        public bool? Vote { get; set; }
        public int Votes { get; set; }
        public RequestRange Range { get; set; }
    }
}
