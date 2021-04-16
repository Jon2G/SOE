using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AndroidX.Core.App;
using SchoolOrganizer.Data;
using SchoolOrganizer.Models.Scheduler;
using SchoolOrganizer.Notifications;
using Xamarin.Forms.Platform.Android;

namespace SchoolOrganizer.Droid.Notifications
{
    public static class ClassAlarm
    {
        public static void ProgramAlarms(Context context)
        {
            List<ClassSquare> timeline = Day.Today()?.GetTimeLine();
            if (timeline is null)
            {
                return;
            }

            NotificationChannel chanel = new NotificationChannel(context, NotificationChannel.ClassChannelId
                , "Notificaciones antes de la hora de tu clase.",
                "Te notificaremos minutos antes de que comience tu clase");
            if ((Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.O)&&!chanel.HasBeenRegistered())
            {
                chanel.RegisterNotificationChannel(context);
            }

            DateTime now = DateTime.Now;
            int ProgrammedId = 1;
            foreach (var cl in timeline)
            {
                DateTime begin = DateTime.Today.Add(cl.Begin);
                DateTime end = DateTime.Today.Add(cl.End);
                end.AddMinutes(-10);
                //bool InProgress = now <= end && begin <= now;
                bool InProgress = (now.Ticks >= begin.Ticks && now <= end);

                Notification notification = new Notification(cl.SubjectName,
                    $"{cl.FormattedTime} ,{cl.Group}\n{(InProgress ? "En curso..." : "Comienza pronto")}",
                    1, cl.Color, context, chanel);

                //si ya inicio enviar una notificación ahora!
                if (InProgress)
                {
                    //notification.Index = 0;
                    //notification.Notify();
                }
                else if (begin > now) //si no ha iniciado se debe programar una alerta
                {
                    Alarm.ProgramFor(notification, begin.AddMinutes(-10), context,ProgrammedId);
                    ProgrammedId++;
                }
                else if (begin < now)
                {
                    //Program for next week
                    DateTime tommorrow = DateTime.Today.AddDays(7).Add(cl.Begin);
                    tommorrow.AddMinutes(-10);
                    Alarm.ProgramFor(notification, tommorrow, context, ProgrammedId);
                    ProgrammedId++;
                }
            }

        }
    }
}