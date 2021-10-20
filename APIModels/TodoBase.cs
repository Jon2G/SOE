using Kit.Model;
using Kit.Sql.Attributes;
using Kit.Sql.Interfaces;
using System;

namespace SOEWeb.Shared
{
    public class TodoBase : ModelBase, IGuid
    {
        [PrimaryKey,AutoIncrement]
        public Guid Guid { get; set; }
        private string _Title;
        public string Title
        {
            get => this._Title;
            set
            {
                this._Title = value;
                this.Raise(() => this.Title);
            }
        }
        private string _Description;
        public string Description
        {
            get => this._Description;
            set
            {
                this._Description = value;
                this.Raise(() => this.Description);
            }
        }
        private DateTime _Date;
        public DateTime Date
        {
            get => this._Date;
            set
            {
                this._Date = value;
                this.Raise(() => this.Date);
            }
        }
        private TimeSpan _Time;
        public TimeSpan Time
        {
            get => this._Time;
            set
            {
                this._Time = value;
                this.Raise(() => this.Time);
            }
        }
        public bool HasPictures { get; set; }

        private Subject _Subject;
        [Ignore]
        public Subject Subject
        {
            get => this._Subject;
            set
            {
                this._Subject = value;
                this.Raise(() => this.Subject);
            }
        }

    }
}
