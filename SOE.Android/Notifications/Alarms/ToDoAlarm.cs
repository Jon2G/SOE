using Android.Content;
using Plugin.CurrentActivity;
using SOE.Models.TodoModels;
using SOE.Notifications;
using System;
using Xamarin.Forms.Internals;

namespace SOE.Droid.Notifications.Alarms
{
    [Preserve]
    public class ToDoAlarm : SOE.Notifications.Alarms.TodoAlarm
    {
        public override void ReSheduleTask(ToDo todo)
        {
            Context context = CrossCurrentActivity.Current.AppContext;
            DateTime date = todo.Date.Add(todo.Time);
            new Notification(todo.Title,
                    $"{todo.Subject.Name} - {todo.Subject.Group}\n{todo.Description}",
                    this.GetProgrammedId(todo), todo.Subject.Color, date, context, this.Channel)
                .Schedule();
        }

        protected override void SetMidnightService() => this.MidnightService();
        protected override IChannel Channel => this.GetChannel();
        protected override Type NotificationType => typeof(Notification);
    }
}