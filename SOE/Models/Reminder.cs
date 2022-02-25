using FirestoreLINQ;
using Google.Cloud.Firestore;
using Kit;
using Kit.Model;
using Kit.Sql.Attributes;
using SOE.API;
using SOE.Data;
using SOE.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SOE.Models
{
    [FirestoreData, FireStoreCollection("Reminders")]
    public class Reminder : ModelBase
    {
        [FirestoreDocumentId]
        public string DocumentId { get; set; }
        private string _Title;
        [FirestoreProperty]
        public string Title
        {
            get => _Title;
            set
            {
                _Title = value;
                Raise(() => Title);
            }
        }

        [FirestoreProperty]
        public Google.Cloud.Firestore.Timestamp GDate
        {
            get => Timestamp.FromDateTime(Date);
            set => Date.ToDateTime();
        }
        public DateTime Date
        {
            get => this.GDate.ToDateTime();
            set
            {
                GDate = Google.Cloud.Firestore.Timestamp.FromDateTime(value);
                Raise(() => Date);
            }
        }
        [FirestoreProperty]
        public int DueTime
        {
            get => Time.ToFirestoreTime();
            set => FireStoreExtensions.ToFirestoreTime(value);
        }
        public TimeSpan Time
        {
            get => FireStoreExtensions.ToFirestoreTime(DueTime);
            set
            {
                DueTime = FireStoreExtensions.ToFirestoreTime(value);
                Raise(() => Time);
            }
        }
        //public Guid SubjectId
        //{
        //    get
        //    {
        //        if (Subject is null)
        //        {
        //            Subject = new Subject();
        //        }
        //        return Subject.Guid;
        //    }
        //    set
        //    {
        //        if (Subject is null)
        //        {
        //            Subject = new Subject();
        //        }
        //        Subject.Guid = value;
        //    }
        //}

        private Subject _Subject;
        [FirestoreProperty]
        public Subject Subject
        {
            get => _Subject;
            set
            {
                _Subject = value;
                Raise(() => Subject);
            }
        }
        private PendingStatus _Status;
        [FirestoreProperty]
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
        [FirestoreProperty]
        public bool IsComplete => Status == PendingStatus.Done;

        [Ignore]
        public string FormattedTime => $"{this.Time:hh}:{this.Time:mm}";
        [Ignore]
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
        public static async Task Save(Reminder reminder)
        {
            await Task.Yield();
            reminder.Date = new DateTime(reminder.Date.Year, reminder.Date.Month, reminder.Date.Day);
            AppData.Instance.LiteConnection.InsertOrReplace(reminder);

        }

        public static CollectionReference Collection =>
            FireBaseConnection.Instance.UserDocument.Collection<Reminder>();

        public static async IAsyncEnumerable<Reminder> Query(Query query)
        {
            QuerySnapshot capitalQuerySnapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
            {
                yield return documentSnapshot.ConvertTo<Reminder>();
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
