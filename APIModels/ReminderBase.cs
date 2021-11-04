using Kit.Model;
using Kit.Sql.Attributes;
using Kit.Sql.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOEWeb.Shared
{
    public class ReminderBase : ModelBase, IGuid
    {
        [PrimaryKey, AutoIncrement]
        public Guid Guid { get; set; }
        private string _Title;
        public string Title
        {
            get => _Title;
            set
            {
                _Title = value;
                Raise(() => Title);
            }
        }
        private DateTime _Date;
        public DateTime Date
        {
            get => _Date;
            set
            {
                _Date = value;
                Raise(() => Date);
            }
        }
        private TimeSpan _Time;
        public TimeSpan Time
        {
            get => _Time;
            set
            {
                _Time = value;
                Raise(() => Time);
            }
        }
        public int SubjectId
        {
            get
            {
                if (Subject is null)
                {
                    Subject = new Subject();
                }
                return Subject.Id;
            }
            set
            {
                if (Subject is null)
                {
                    Subject = new Subject();
                }
                Subject.Id = value;
            }
        }
        [JsonIgnore]
        private Subject _Subject;
        [Ignore]
        public Subject Subject
        {
            get => _Subject;
            set
            {
                _Subject = value;
                Raise(() => Subject);
            }
        }

    }
}
