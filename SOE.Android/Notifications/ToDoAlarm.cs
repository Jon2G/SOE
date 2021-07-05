using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Plugin.CurrentActivity;
using SOE.Models.Scheduler;
using SOE.Models.TaskFirst;
using SOE.Widgets;

namespace SOE.Droid.Notifications
{
    public static class ToDoAlarm
    {
        public static void ReSheduleTask(ToDo todo)
        {
            Context context = CrossCurrentActivity.Current.AppContext;
            var channel = GetChannel(context);
            DateTime date = todo.Date.Add(todo.Time);
            Notification notification = new Notification(todo.Title,
                $"{todo.Subject.Name} - {todo.Subject.Group}\n{todo.Description}",
                1, todo.Subject.Color, context, channel);
            int programmedId = GetProgrammedId(todo);
            Alarm.ProgramFor(notification, date.AddDays(-1), context, programmedId);
        }
        public static void ProgramAlarms(Context context)
        {
            List<ToDo> todos = ToDosWidget.GetTasks();
            if (todos is null)
            {
                return;
            }
            var channel = GetChannel(context);
            foreach (ToDo todo in todos.Where(x => ToDo.DaysLeft(x) > 0))
            {
                DateTime date = todo.Date.Add(todo.Time);
                Notification notification = new Notification(todo.Title,
                    $"{todo.Subject.Name} - {todo.Subject.Group}\n{todo.Description}",
                    1, todo.Subject.Color, context, channel);
                int programmedId = GetProgrammedId(todo);
                Alarm.ProgramFor(notification, date.AddDays(-1), context, programmedId);
            }

            Bundle extras = new Bundle(1);
            extras.PutInt(nameof(Notification.MidnightCode), Notification.MidnightCode);
            Alarm.ProgramFor(extras, DateTime.Today.AddDays(1).AddMinutes(-1), context, Notification.MidnightCode);

        }

        private static NotificationChannel GetChannel(Context context=null)
        {
            context ??= CrossCurrentActivity.Current.AppContext;
            NotificationChannel channel = new NotificationChannel(context, NotificationChannel.ToDoChannelId
                , "Notificaciones dias antes de la entrega de tus actividades.",
                "Te notificaremos y recordaremos sobre tus actividades programadas");
            if ((Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.O) && !channel.HasBeenRegistered())
            {
                channel.RegisterNotificationChannel(context);
            }
            return channel;
        }

        private static int GetProgrammedId(ToDo todo) =>
            Convert.ToInt32($"{Notification.ClassTimeCode}{todo.Subject.Id}{todo.Id}");
    }
}