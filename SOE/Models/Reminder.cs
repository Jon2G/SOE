using FirestoreLINQ;
using Kit;
using Kit.Model;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
using SOE.API;
using SOE.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SOE.Models
{
    [FireStoreCollection("Reminders")]
    public class Reminder : ModelBase
    {
        [Id]
        public string DocumentId { get; set; }
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
        [Ignored]
        public TimeSpan Time
        {
            get => _Time;
            set
            {
                _Time = value;
                Raise(() => Time);
            }
        }
        public double TotalHours
        {
            get => Time.TotalHours;
            set
            {
                if (Time.TotalHours != value)
                {
                    Time = TimeSpan.FromHours(value);
                }
            }
        }

        private Subject _Subject;
        [Ignored]
        public Subject Subject
        {
            get => this._Subject;
            set
            {
                this._Subject = value;
                this.SubjectId = value?.GetDocumentId();
                this.Raise(() => this.Subject);
            }
        }
        public string? SubjectId { get; set; }
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
        [Ignored]
        public bool IsComplete => Status == PendingStatus.Done;

        [Ignored]
        public string FormattedTime => $"{this.Time:hh}:{this.Time:mm}";
        [Ignored]
        public string FormattedDate => $"{this.Date.DayOfWeek.GetDayName()} - {this.Date:dd/MM}";
        //public Reminder(string Title, DateTime dateTime)
        //{
        //    this.Title = Title;
        //    this.Date = dateTime;
        //}
        public Reminder()
        {
            this.Date = DateTime.Today;
        }

        public Task Delete() => Collection.Document(this.DocumentId)?.DeleteAsync() ?? Task.CompletedTask;

        public async Task<Reminder> Save()
        {
            await Task.Yield();
            if (string.IsNullOrEmpty(this.DocumentId))
            {
                Date = new DateTime(Date.Year, Date.Month, Date.Day);
                await Collection.AddAsync(this);
                return this;
            }
            await Collection.Document(DocumentId).SetAsync(this);
            return this;
        }

        public static ICollectionReference Collection =>
            FireBaseConnection.UserDocument.Collection<Reminder>();

        public static async IAsyncEnumerable<Reminder> IQuery(IQuery IQuery)
        {
            IQuerySnapshot capitalQuerySnapshot = await IQuery.GetAsync();
            foreach (IDocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
            {
                yield return documentSnapshot.ToObject<Reminder>();
            }
        }
        internal static async Task<string> ShareReminder(Reminder reminder)
        {
            await Task.Yield();
            throw new NotImplementedException();
            //if (reminder.Subject is not null && reminder.Subject.IsOffline)
            //{
            //    if (!await reminder.Subject.Sync(AppData.Instance, new SyncService()))
            //    {
            //        App.Current.MainPage.DisplayAlert("Opps...",
            //            "No fue posible compartir este recordatorio, revise su conexión a internet", "Ok").SafeFireAndForget();
            //        return null;
            //    }
            //}
            //Response Response = await APIService.Current.PostReminder(reminder);
            //if (Response.ResponseResult != APIResponseResult.OK)
            //{
            //    App.Current.MainPage.DisplayAlert("Opps...", "No fue posible compartir este recordatorio, revise su conexión a internet", "Ok").SafeFireAndForget();
            //    return null;
            //}
            //return DynamicLinkFormatter.GetDynamicUrl("share",
            //    new Dictionary<string, string>() { { "type", "reminder" }, { "id", reminder.Guid.ToString("N") } });
        }

    }
}
