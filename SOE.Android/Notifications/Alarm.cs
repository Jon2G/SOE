﻿using System;
using Acr.UserDialogs.Builders;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Kit.Droid;
using SOE.Notifications;
using Exception = Java.Lang.Exception;

[assembly: UsesPermission(Name = Manifest.Permission.WakeLock)]
namespace SOE.Droid.Notifications
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
            if (intent?.HasExtra(nameof(Notification)) ?? false)
            {
                Notification notification = Notification.FromExtras(intent.Extras, context);
                notification.Notify();
            }

            try
            {
                //if (Build.VERSION.SdkInt < BuildVersionCodes.Q)
                //{
                if (!context.IsServiceRunning(typeof(NotificationService)))
                {
                    context.StartService(new Intent(context, typeof(NotificationService)));
                }
                //}
                //else
                //{
                //    context.StartForegroundService(new Intent(context, typeof(NotificationService)));
                //}

            }
            catch (Exception)
            {
                NotificationChannel chanel = NotificationChannel.GetNotificationChannel(context, NotificationChannel.ClassChannelId);
                chanel?.Notify("OnReceive", $"{intent?.Action}");
            }
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