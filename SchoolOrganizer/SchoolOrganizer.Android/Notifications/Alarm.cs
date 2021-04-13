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
using Android;
using AndroidX.Core.App;
using Java.Lang;
using Kit.Droid;
using Plugin.CurrentActivity;
using SchoolOrganizer.Data;
using SchoolOrganizer.Droid.Notifications;
using SchoolOrganizer.Models.Scheduler;
using SchoolOrganizer.Notifications;
using SchoolOrganizer.Widgets;
using Android.Graphics;
using Xamarin.Forms.Platform.Android;

[assembly: UsesPermission(Name = Manifest.Permission.WakeLock)]
[assembly: Xamarin.Forms.Dependency(typeof(Alarm))]
namespace SchoolOrganizer.Droid.Notifications
{
    [BroadcastReceiver(Exported = true, Enabled = true)]
    [IntentFilter(new[] { LocalNotifications.START_ALARM })]
    public class Alarm : BroadcastReceiver, ILocalNotificationService
    {
        public const string NotificationChannelId = "NotificationService_SchoolOrganizer";
        private Day Day;
        private List<ClassSquare> TimeLine;
        public Alarm()
        {
            CreateNotificationChannel(CrossCurrentActivity.Current.AppContext);
        }
        private void CreateNotificationChannel(Context context)
        {
            // Create the NotificationChannel, but only on API 26+ because
            // the NotificationChannel class is new and not in the support library
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                var name = "Notificación antes de la hora de tu clase"; //GetString(Android.Resource.String.channel_name);
                string description = "Te notificaremos minutos antes de que comience tu clase";//GetString(Android.Resource.String.channel_description);
                var importance = Android.App.NotificationImportance.Default;
                NotificationChannel channel = new NotificationChannel(NotificationChannelId, name, importance);
                channel.Description = description;
                // Register the channel with the system; you can't change the importance
                // or other notification behaviors after this
                NotificationManager notificationManager = (NotificationManager)context.
                    GetSystemService(Java.Lang.Class.FromType(typeof(NotificationManager)));
                notificationManager.CreateNotificationChannel(channel);
            }
        }
        public override void OnReceive(Context context, Intent intent)
        {
            if (AppData.Instance is null)
            {
                AppData.Init();
            }

            Day = Day.Today();
            TimeLine = Day.GetTimeLine();
            NotificationsHistory.Clear();

            if (Day.DayOfWeek != DateTime.Today.DayOfWeek)
            {
                Day = Day.Today();
                TimeLine = Day.GetTimeLine();
            }

            int NotificationIndex = 0;
            foreach (var cl in TimeLine)
            {
                var diff = cl.Begin - DateTime.Now.TimeOfDay;
                if (diff.Minutes >= 0 && diff.Minutes <= 10)
                {
                    if (!NotificationsHistory.HasBeenNotified(DateTime.Today, NotificationType.Class, cl.Group))
                    {
                        NotifyNextClass(NotificationIndex, context, cl);
                        NotificationIndex++;
                    }
                    continue;
                }
                if (diff.Minutes > 0)
                {
                    diff = diff - TimeSpan.FromMinutes(5);
                    Start(context, (long)diff.TotalMilliseconds);
                    return;
                }
            }
            //Todas las clases para hoy ya han pasado...
            if (TimeLine.FirstOrDefault() is ClassSquare tomorrowFirstClass)
            {
                DateTime tommorowdate = DateTime.Today.AddDays(1).Add(tomorrowFirstClass.Begin);
                TimeSpan timeUntilTomorrow = tommorowdate - DateTime.Now;
                Start(context, (long)timeUntilTomorrow.TotalMilliseconds);
            }
            //Start(context); //retick

        }

        private void NotifyNextClass(int notificationIndex, Context context, ClassSquare cl)
        {
            PowerManager pm = (PowerManager)context.GetSystemService(Context.PowerService);
            PowerManager.WakeLock wl = pm.NewWakeLock(WakeLockFlags.Partial, "");
            wl.Acquire();
            var color = Xamarin.Forms.Color.FromHex(cl.Color).ToAndroid();
            NotificationCompat.Builder builder = new NotificationCompat.Builder(context, NotificationChannelId)
                .SetSmallIcon(Android.Resource.Id.Icon)
                .SetContentTitle("Comienza pronto")
                .SetVisibility(NotificationCompat.VisibilityPublic)
                .SetContentText(cl.SubjectName)
                .SetStyle(new NotificationCompat.BigTextStyle()
                    .BigText($"{cl.FormattedTime} , {cl.Group}")
                    .SetBigContentTitle(cl.SubjectName))
                .SetPriority(NotificationCompat.PriorityHigh)
                .SetColorized(true)
                .SetColor(color)
                .SetVibrate(new long[] { 0, 1000, 200, 1000 })
                .SetLights(color, 300, 100);


            NotificationManagerCompat notificationManager = NotificationManagerCompat.From(context);
            // notificationId is a unique int for each notification that you must define
            notificationManager.Notify(notificationIndex, builder.Build());
            wl.Release();

            NotificationsHistory notification = new NotificationsHistory()
            {
                NotificationType = NotificationType.Class,
                ObjectId = cl.Group,
                Date = DateTime.Today
            };
            notification.Save();

        }

        public void Start() => Start(CrossCurrentActivity.Current.AppContext);
        public void Cancel() => Cancel(CrossCurrentActivity.Current.AppContext);

        public void Start(Context context)
        {
            TimeSpan interval = new TimeSpan(0, 0, 1, 0);
            Start(context, interval.Milliseconds);
        }
        public void Start(Context context, long MillisUntilNextEffectiveTick)
        {
            AlarmManager am = (AlarmManager)context.GetSystemService(Context.AlarmService);
            Intent i = new Intent(context, typeof(Alarm));
            PendingIntent pi = PendingIntent.GetBroadcast(context, 0, i, 0);
            am.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, JavaSystem.CurrentTimeMillis() + MillisUntilNextEffectiveTick, pi);
            //am.SetInexactRepeating(AlarmType.RtcWakeup, JavaSystem.CurrentTimeMillis(), interval.Milliseconds, pi); // Millisec * Second * Minute
            // am.SetRepeating(AlarmType.RtcWakeup, JavaSystem.CurrentTimeMillis(), 1000 * 60 * 10, pi); // Millisec * Second * Minute 
        }
        public void Cancel(Context context)
        {
            Intent intent = new Intent(context, typeof(Alarm));
            PendingIntent sender = PendingIntent.GetBroadcast(context, 0, intent, 0);
            AlarmManager alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);
            alarmManager.Cancel(sender);
        }
    }
}