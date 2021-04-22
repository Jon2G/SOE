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
        public override void OnReceive(Context context, Intent intent)
        {
            if (context is null)
            {
                return;
            }
            //NotificationChannel chanel = NotificationChannel.GetNotificationChannel(context, NotificationChannel.ClassChannelId);
            //chanel?.Notify("OnReceive", $"{intent?.Action}");
            if (intent?.HasExtra(nameof(Notification)) ?? false)
            {
                Notification notification = Notification.FromExtras(intent.Extras, context);
                notification.Notify();
            }
            context.StartForegroundService(new Intent(context, typeof(NotificationService)));
        }

        internal static void ProgramFor(Bundle extras, DateTime date, Context context, int requestId)
        {
            long trigger_milis = date.ToUniversalTime().ToUnixTimestamp();
            Intent i = new Intent(context, typeof(Alarm));
            i.PutExtras(extras);
            PendingIntent pi = PendingIntent.GetBroadcast(context, requestId, i, 0);
            AlarmManager am = (AlarmManager)context.GetSystemService(Context.AlarmService);
            am.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, trigger_milis, pi);
        }
        internal static void ProgramFor(Notification notification, DateTime date, Context context, int requestId)
        {
            ProgramFor(notification.ToExtras(), date, context, requestId);
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