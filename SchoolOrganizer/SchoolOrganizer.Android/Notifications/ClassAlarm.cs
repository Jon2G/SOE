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
    public class ClassAlarm : NotificationBase
    {

        private readonly Alarm Alarm;
        public ClassAlarm(Alarm Alarm, Context Context) : base(Context)
        {
            this.Alarm = Alarm;

        }

        public void Start()
        {
            if (AppData.Instance is null)
            {
                AppData.Init();
            }
            var day = Day.Today();
            var TimeLine = day.GetTimeLine();
            NotificationsHistory.Clear();
            int NotificationIndex = 1;
            var now = DateTime.Now;
            foreach (var cl in TimeLine)
            {
                var begin_date = DateTime.Today.Add(cl.Begin);
                var end_date = DateTime.Today.Add(cl.End);

                var diff = begin_date - now;
                if (diff.TotalMinutes <= 0 && end_date > now)
                {
                    if ((end_date - now).TotalMinutes >= 10)
                    {
                        //En curso
                        Notify(NotificationIndex, cl, true);
                        NotificationIndex++;
                    }
                    continue;
                }
                if (diff.TotalMinutes >= 0 && diff.TotalMinutes <= 10)
                {
                    //if (!NotificationsHistory.HasBeenNotified(DateTime.Today, NotificationType.Class, cl.Group))
                    //{
                    Notify(NotificationIndex, cl);
                    NotificationIndex++;
                    //}
                    continue;
                }
                if (diff.TotalMinutes > 0)
                {
                    begin_date = begin_date - TimeSpan.FromMinutes(10);
                    this.Alarm.Start(begin_date);
                    return;
                }
            }
            //Todas las clases para hoy ya han pasado...
            if (TimeLine.FirstOrDefault() is ClassSquare tomorrowFirstClass)
            {
                DateTime tommorowdate = DateTime.Today.AddDays(1).Add(tomorrowFirstClass.Begin);
                tommorowdate = tommorowdate - TimeSpan.FromMinutes(10);
                this.Alarm.Start(tommorowdate);
            }
            //Start(context); //retick
        }


        private void Notify(int notificationIndex, ClassSquare cl, bool InProgress = false)
        {
            NotificationChannel chanel = new NotificationChannel(this.Context, NotificationChannel.ClassChannelId
                , "Notificaciones antes de la hora de tu clase.", "Te notificaremos minutos antes de que comience tu clase");
            string content =
                $"{cl.FormattedTime} , {cl.Group}\n{(InProgress ? "En estos momentos" : "Comienza pronto")}";
            var color = Xamarin.Forms.Color.FromHex(cl.Color).ToAndroid();
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this.Context, chanel.ChannelId)
                .SetSmallIcon(Android.Resource.Id.Icon)
                .SetContentTitle(cl.SubjectName)
                .SetContentText(content)
                .SetStyle(new NotificationCompat.BigTextStyle()
                    .BigText(content)
                    .SetBigContentTitle(cl.SubjectName))
                .SetColorized(true)
                .SetColor(color);
            //chanel.NotificationManager.Notify(notificationIndex, builder.Build());
            Notify(notificationIndex, builder, chanel);
        }
    }
}