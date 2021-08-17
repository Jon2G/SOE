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

            DateTime now = DateTime.Now;
            foreach (ClassSquare cl in timeline)
            {
                DateTime begin = day.Date.Add(cl.Begin);
                DateTime end = day.Date.Add(cl.End).AddMinutes(-10);
                //bool InProgress = now <= end && begin <= now;
                bool InProgress = (now.Ticks >= begin.Ticks && now <= end);

                int programmedId = Convert.ToInt32($"{Notification.ClassTimeCode}{cl.Subject.Id}{(int)day.DayOfWeek}");
                DateTime desiredDate = DateTime.MinValue;

                //si ya inicio enviar una notificación ahora!
                if (InProgress)
                {
                    //notification.Index = 0;
                    //notification.Notify();
                }
                else if (begin > now) //si no ha iniciado se debe programar una alerta
                {
                    desiredDate = begin.AddMinutes(-10);
                }
                else if (begin < now)
                {
                    //Program for next week
                    desiredDate = day.Date.AddDays(7).Add(cl.Begin).AddMinutes(-10);
                }
                Notification notification = new(cl.Subject.Name,
                    $"{cl.FormattedTime} ,{cl.Subject.Group}\n{(InProgress ? "En curso..." : "Comienza pronto")}",
                    1, cl.Subject.Color, desiredDate, context, chanel);
                Alarm.ProgramFor(notification, desiredDate, context, programmedId);
            }

            Bundle extras = new Bundle(1);
            extras.PutInt(nameof(Notification.MidnightCode), Notification.MidnightCode);
            Alarm.ProgramFor(extras, day.Date.AddDays(1), context, Notification.MidnightCode);

        }
    }
}