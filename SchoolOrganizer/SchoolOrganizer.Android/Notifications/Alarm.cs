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
using Acr.UserDialogs.Builders;
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
using Android.Icu.Util;
using TimeZone = Android.Icu.Util.TimeZone;

[assembly: UsesPermission(Name = Manifest.Permission.WakeLock)]
namespace SchoolOrganizer.Droid.Notifications
{
    [BroadcastReceiver(Exported = true, Enabled = true)]
    [IntentFilter(new[] { LocalNotifications.START_ALARM })]
    public class Alarm : BroadcastReceiver
    {
        //public const string NotificationChannelId = "NotificationService_SchoolOrganizer";
        private Context _Context;
        private Context Context
        {
            get => _Context ??= GetAppContext();
            set => _Context = value;
        }
        public Alarm()
        {

        }
        
        public override void OnReceive(Context context, Intent intent)
        {
            this.Context = GetAppContext()??context;
            NotificationChannel chanel =NotificationChannel.GetNotificationChannel(this.Context, NotificationChannel.ClassChannelId);
            chanel?.Notify("OnReceive", "OnReceive",2);
            ClassAlarm ClassAlarm = new ClassAlarm(this, this.Context);
            ClassAlarm.Start();


        }


        public void Start(Context context)
        {
            this.Context = context;
            NotificationChannel chanel = NotificationChannel.GetNotificationChannel(this.Context, NotificationChannel.ClassChannelId);
            chanel?.Notify("Start", "Start",3);
            OnReceive(context, null);
        }

        public void Start(DateTime date)
        {
           
            // every day at scheduled time 
            //Calendar calendar = Calendar.GetInstance(Android.Icu.Util.TimeZone.GetTimeZone("UTC"));
            //calendar.Set(CalendarField.DayOfMonth, date.Day);
            //calendar.Set(CalendarField.Month, date.Month);
            //calendar.Set(CalendarField.Year, date.Year);
            //calendar.Set(CalendarField.HourOfDay, date.Hour);
            //calendar.Set(CalendarField.Minute, date.Minute);
            //calendar.Set(CalendarField.Second, 0);

            Intent i = new Intent(this.Context, typeof(Alarm));
            PendingIntent pi = PendingIntent.GetBroadcast(this.Context, 0, i, 0);

            AlarmManager am = (AlarmManager)this.Context.GetSystemService(Context.AlarmService);
            am.SetExact(AlarmType.RtcWakeup, date.ToUniversalTime().ToUnixTimestamp(), pi);

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
        public static Context GetAppContext()
        {
            return Android.App.Application.Context;
        }
    }
}