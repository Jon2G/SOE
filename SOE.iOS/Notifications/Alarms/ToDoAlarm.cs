using Foundation;
using SOE.Models.TodoModels;
using SOE.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace SOE.iOS.Notifications.Alarms
{
    [Preserve]
    public class ToDoAlarm : SOE.Notifications.Alarms.TodoAlarm
    {
        protected override Type NotificationType => typeof(Notification);
        public override void ReSheduleTask(ToDo todo)
        {
            uint index = this.GetProgrammedId(todo);
            UIApplication.SharedApplication.ScheduledLocalNotifications
                .FirstOrDefault(x => x.MatchIndex(index))?.Cancel();
            DateTime date = todo.Date.Add(todo.Time);
            date = date.AddDays(-1);
            new Notification()
                .Set(todo.Title,
                    $"{todo.Subject.Name} - {todo.Subject.Group}\n{todo.Description}",
                    index, Xamarin.Forms.Color.FromHex(todo.Subject.Color), date, this.Channel, "ToDo")
                .Schedule();
        }
    }
}