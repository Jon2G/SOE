using Android.Content;
using Plugin.CurrentActivity;
using SOE.Models.TodoModels;
using SOE.Notifications;
using System;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace SOE.Droid.Notifications.Alarms
{
    [Preserve]
    public class ToDoAlarm : SOE.Notifications.Alarms.TodoAlarm
    {
        public override async Task ReSheduleTask(ToDo todo)
        {
            await Task.Yield();
            Context context = CrossCurrentActivity.Current.AppContext;
            DateTime date = todo.Date.Add(todo.Time);
            if (todo is null)
            {
                return;
            }
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
            new Notification(todo.Title,
                    $"{todo.Subject.Name} - {todo.Subject.Group.Name}\n{todo.Description}",
                    this.GetProgrammedId(todo), todo.Subject.Color, date, context, channel)
                .Schedule();
        }

        protected override void SetMidnightService() => this.MidnightService();
        protected override IChannel? Channel => this.GetChannel();
        protected override Type NotificationType => typeof(Notification);
    }
}