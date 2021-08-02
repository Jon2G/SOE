using System;
using SOEWeb.Shared;
using Kit;
using Kit.Model;
using Kit.Sql.Attributes;
using Kit.Sql.Interfaces;
using SOE.Data;
using System.Windows.Input;
using Xamarin.Forms;
using SOE.Enums;
using SOE.API;
using SOEWeb.Shared.Enums;
using AsyncAwaitBestPractices;
using System.Threading.Tasks;

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
        private PendingStatus _Status;

        public PendingStatus Status
        {
            get => _Status;
            set
            {
                _Status = value;
                Raise(() => Status);
                Raise(() => IsComplete);
            }
        }

        public bool IsComplete => Status == PendingStatus.Done;

        [Ignore]
        public string FormattedTime => $"{this.Time:hh}:{this.Time:mm}";
        [Ignore]
        public string FormattedDate => $"{this.Date.DayOfWeek.Dia()} - {this.Date:dd/MM}";
        //public Reminder(string Title, DateTime dateTime)
        //{
        //    this.Title = Title;
        //    this.Date = dateTime;
        //}
        public Reminder()
        {
            this.Date = DateTime.Today;
        }
        public static async Task Save(Reminder reminder)
        {
            await Task.Yield();
            reminder.Date = new DateTime(reminder.Date.Year, reminder.Date.Month, reminder.Date.Day);
            AppData.Instance.LiteConnection.InsertOrReplace(reminder);

        }
        internal static async Task<string> ShareR(Reminder reminder)
        {
            Response Response = await APIService.PostReminder(reminder);
            if (Response.ResponseResult != APIResponseResult.OK)
            {
                App.Current.MainPage.DisplayAlert("Opps...", Response.Message, "Ok").SafeFireAndForget();
                return null;
            }
            return $"{APIService.Url}/{APIService.ShareReminder}/{reminder.Guid:N}";

        }

    }
}
