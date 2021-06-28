using System;
using System.Collections.Generic;
using System.Text;
using Kit.Sql.Attributes;
using Kit.Sql.Interfaces;
using SchoolOrganizer.Data;
using SchoolOrganizer.Models.Scheduler;

namespace SchoolOrganizer.Notifications
{

    public class NotificationsHistory : IGuid
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string ObjectId { get; set; }
        public NotificationType NotificationType { get; set; }
        public DateTime Date { get; set; }

        public NotificationsHistory()
        {

        }

        public static bool HasBeenNotified(DateTime date, NotificationType notificationType, string ObjectId)
        {
            return AppData.Instance.LiteConnection
                .Table<NotificationsHistory>()
                .FirstOrDefault(x => x.Date == date && x.NotificationType == notificationType && x.ObjectId == ObjectId) != null;
        }

        public void Save()
        {
            AppData.Instance.LiteConnection.Insert(this);
        }

        public static void Clear()
        {
            //AppData.Instance.LiteConnection.DeleteAll<NotificationsHistory>();
            AppData.Instance.LiteConnection.Table<NotificationsHistory>()
                .Where(x => x.Date != DateTime.Today)
                .ToList().ForEach(y => AppData.Instance.LiteConnection.Delete(y));

        }
    }
}
