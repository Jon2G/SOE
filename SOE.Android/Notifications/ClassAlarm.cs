using System;
using System.Collections.Generic;
using Android.Content;
using Android.OS;
using SOE.Models.Scheduler;

namespace SOE.Droid.Notifications
{
    public static class ClassAlarm
    {
        public static void ProgramAlarms(Context context)
        {
            Day day = Day.Today();
            List<ClassSquare> timeline = day?.GetTimeLine();
            if (timeline is null)
            {
                return;
            }

            NotificationChannel chanel = new NotificationChannel(context, NotificationChannel.ClassChannelId
                , "Notificaciones antes de la hora de tu clase.",
                "Te notificaremos minutos antes de que comience tu clase");
            if ((Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.O) && !chanel.HasBeenRegistered())
            {
                chanel.RegisterNotificationChannel(context);
            }

            DateTime now = day.Date;
            foreach (ClassSquare cl in timeline)
            {
                DateTime begin = day.Date.Add(cl.Begin);
                DateTime end = day.Date.Add(cl.End).AddMinutes(-10);
                //bool InProgress = now <= end && begin <= now;
                bool InProgress = (now.Ticks >= begin.Ticks && now <= end);

                Notification notification = new Notification(cl.Subject.Name,
                    $"{cl.FormattedTime} ,{cl.Subject.Group}\n{(InProgress ? "En curso..." : "Comienza pronto")}",
                    1, cl.Subject.Color, context, chanel);

                int programmedId = Convert.ToInt32($"{Notification.ClassTimeCode}{cl.Subject.Id}{(int)day.DayOfWeek}");
                //si ya inicio enviar una notificación ahora!
                if (InProgress)
                {
                    //notification.Index = 0;
                    //notification.Notify();
                }
                else if (begin > now) //si no ha iniciado se debe programar una alerta
                {
                    Alarm.ProgramFor(notification, begin.AddMinutes(-10), context, programmedId);
                }
                else if (begin < now)
                {
                    //Program for next week
                    DateTime tommorrow = day.Date.AddDays(7).Add(cl.Begin).AddMinutes(-10);
                    Alarm.ProgramFor(notification, tommorrow, context, programmedId);
                }
            }

            Bundle extras = new Bundle(1);
            extras.PutInt(nameof(Notification.MidnightCode), Notification.MidnightCode);
            Alarm.ProgramFor(extras, day.Date.AddDays(1), context, Notification.MidnightCode);

        }
    }
}