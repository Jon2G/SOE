using Foundation;
using SOE.Models.TodoModels;
using SOE.Notifications;
using System;
using System.Linq;
using System.Threading.Tasks;
using UIKit;

namespace SOE.iOS.Notifications.Alarms
{
    [Preserve]
    public class ToDoAlarm : SOE.Notifications.Alarms.TodoAlarm
    {
        protected override Type NotificationType => typeof(Notification);
        public override async Task ReSheduleTask(ToDo todo)
        {
            await Task.Yield();
            if (todo is null)
            {
                return;
            }
            uint index = this.GetProgrammedId(todo);
            UIApplication.SharedApplication.ScheduledLocalNotifications
                .FirstOrDefault(x => x.MatchIndex(index))?.Cancel();
            DateTime date = todo.Date.Add(todo.Time);
            date = date.AddDays(-1);


            await todo.GetSubject();
            if (todo.Subject is null)
            {
                return;
            }
            await todo.Subject.GetGroup();
            if (todo.Subject.Group is null)
            {
                return;
            }
            IChannel? channel = this.Channel;
            if (channel is null) { return; }
            new Notification()
                .Set(todo.Title,
                    $"{todo.Subject.Name} - {todo.Subject.Group.Name}\n{todo.Description}",
                    index, Xamarin.Forms.Color.FromHex(todo.Subject.Color), date, channel, "ToDo")
                .Schedule();
        }
    }
}