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
using Android.Telephony.Gsm;
using Java.Security;
using Kit.Droid.Services;
using Plugin.CurrentActivity;
using SchoolOrganizer.Droid.Notifications;
using SchoolOrganizer.Notifications;
[assembly: Xamarin.Forms.Dependency(typeof(NotificationService))]
namespace SchoolOrganizer.Droid.Notifications
{
    [Service(Enabled = true, Exported = true)]
    [IntentFilter(new[] { ".NotificationService" })]
    public class NotificationService : Service
    {
        public override IBinder? OnBind(Intent? intent)
        {
            return null;
        }
        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            var Context = MainActivity.GetAppContext();
            NotificationChannel chanel = NotificationChannel.GetNotificationChannel(Context, NotificationChannel.ClassChannelId);
            chanel?.Notify("NotificationService.OnStartCommand", intent?.Action ?? "No action");
            return StartCommandResult.Sticky;
        }
        public override void OnCreate()
        {
            var Context = MainActivity.GetAppContext();
            NotificationChannel chanel = NotificationChannel.GetNotificationChannel(Context, NotificationChannel.ClassChannelId);
            chanel?.Notify("NotificationService.OnCreate", "No action");
            //start a separate thread and start listening to your network object
            ClassAlarm.ProgramAlarms(Context);
            chanel?.Notify("OnReceive", "OnReceive");
            base.OnCreate();
        }
    }
}