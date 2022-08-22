using AsyncAwaitBestPractices;
using Forms9Patch.iOS;
using Foundation;
using SOE.iOS.Notifications;
using SOE.iOS.Notifications.Alarms;
using SOE.Models.TodoModels;
using SOE.Notifications;
using System.Collections.Generic;
using System.Linq;
using UIKit;
using UserNotifications;
[assembly: Xamarin.Forms.Dependency(typeof(NotificationService))]
namespace SOE.iOS.Notifications
{
    [Preserve]
    public class NotificationService : ILocalNotificationService
    {

        public void UnScheduleAll()
        {
            UNUserNotificationCenter.Current.RemoveAllPendingNotificationRequests();
            UIApplication app = UIApplication.SharedApplication;
            List<UILocalNotification> notifications = app.ScheduledLocalNotifications.ToList();
            foreach (UILocalNotification notification in notifications)
            {
                NSObject nsIndex = notification.UserInfo?[nameof(Notification.Index)];
                object nsObject = nsIndex?.ToObject();
                if (nsObject is uint index)
                {
                    app.CancelLocalNotification(notification);
                }
            }
        }
        public void ScheduleAll()
        {
            ClassAlarm alarm = new();
            alarm.ScheduleAll().SafeFireAndForget();

            ToDoAlarm todo = new();
            todo.ScheduleAll().SafeFireAndForget();
        }

        public void ReSheduleTask(ToDo toDo)
        {
            ToDoAlarm alarm = new ToDoAlarm();
            alarm.ReSheduleTask(toDo).SafeFireAndForget();
        }
    }
}