using System;
using System.Collections.Generic;
using System.Text;
using Kit.Model;
using Kit.Sql.Attributes;
using Kit.Sql.Interfaces;

namespace APIModels
{
    public class TodoBase : ModelBase, IGuid
    {
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
        private string _Description;
        public string Description
        {
            get => _Description;
            set
            {
                _Description = value;
                Raise(() => Description);
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
