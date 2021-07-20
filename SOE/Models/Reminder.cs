using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using APIModels;
using Kit;
using Kit.Model;
using Kit.Sql.Attributes;
using Kit.Sql.Interfaces;
using SOE.Data;

namespace SOE.Models
{
    public class Reminder : ModelBase, IGuid
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
        [Ignore]
        public string FormattedTime => $"{this.Time:hh}:{this.Time:mm}";
        [Ignore]
        public string FormattedDate => $"{this.Date.DayOfWeek.Dia()} - {this.Date:dd/MM}";
        public Reminder(string Title, DateTime dateTime)
        {
            this.Title = Title;
            this.Date = dateTime;
        }
        public Reminder()
        {
        }
        public static void Save(Reminder reminder)
        {
            reminder.Date = new DateTime(reminder.Date.Year, reminder.Date.Month, reminder.Date.Day);
            AppData.Instance.LiteConnection.InsertOrReplace(reminder);

        }
    }
}
