using Kit.Sql.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOEWeb.Shared
{
    public class SubjectComments
    {
        [PrimaryKey, AutoIncrement, NotNull]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        [Ignore]
        public string UserName { get; set; }
        [MaxLength(140)]
        public string Text { get; set; }
        [Ignore]
        public List<SubjectComments> Comments { get; set; }
    }
}
