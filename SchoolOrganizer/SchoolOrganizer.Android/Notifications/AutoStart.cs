﻿using Android.App;
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
[assembly: UsesPermission(Name = Manifest.Permission.ReceiveBootCompleted)]
namespace SchoolOrganizer.Droid.Notifications
{
    [BroadcastReceiver(Exported = true, Enabled = true)]
    [IntentFilter(new[]
    {
        Intent.ActionBootCompleted, Intent.ActionLockedBootCompleted, Intent.ActionReboot,
        "android.intent.action.QUICKBOOT_POWERON","com.htc.intent.action.QUICKBOOT_POWERON"
    })]
    public class AutoStart : BroadcastReceiver
    {
        public AutoStart()
        {
        }
        public override void OnReceive(Context context, Intent intent)
        {
            //if (intent.Action == Intent.ActionBootCompleted)

            //chanel.Notify("AutoStart.OnReceive", intent?.Action ?? "No action", 4);
            if (context != null)
            {
                //chanel.Notify("AutoStart.OnReceive", "Service fired", 5);
                try
                {
                    Intent myIntent = new Intent(context, Java.Lang.Class.FromType(typeof(NotificationService)));
                    context.StartForegroundService(myIntent);
                }
                catch (Exception ex)
                {
                    NotificationChannel chanel = NotificationChannel.GetNotificationChannel(context, NotificationChannel.ClassChannelId);
                    chanel?.Notify("Exception", ex?.Message??"No message", 6);
                }

            }
            else
            {
                NotificationChannel chanel = NotificationChannel.GetNotificationChannel(context, NotificationChannel.ClassChannelId);
                chanel?.Notify("AutoStart.OnReceive", "context is null", 5);
            }


            //Intent startService = new Intent(context,
            //    typeof(NotificationService));
            //startService.SetPackage(context.PackageName);
            //PendingIntent restartServicePI = PendingIntent.GetService(context, 0, startService, PendingIntentFlags.OneShot);
            //Start the service once it has been killed android
            //AlarmManager alarmService = (AlarmManager)context.GetSystemService(Context.AlarmService);
            //alarmService.Set(AlarmType.ElapsedRealtime, SystemClock.ElapsedRealtime() + 100, restartServicePI);


        }
    }
}