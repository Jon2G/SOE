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
using Plugin.CurrentActivity;
using SchoolOrganizer.Droid.Notifications;
using SchoolOrganizer.Notifications;
using Kit.Droid.Services;

[assembly: Xamarin.Forms.Dependency(typeof(NotificationService))]
namespace SchoolOrganizer.Droid.Notifications
{
    [Service(Enabled = true, Exported = true)]
    [IntentFilter(new[] { ".NotificationService" })]
    public class NotificationService : Service
    {
        public override IBinder? OnBind(Intent? intent) => null;
        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            base.OnStartCommand(intent, flags, startId);
            return StartCommandResult.Sticky;
        }
        public override void OnCreate()
        {
            var Context = MainActivity.GetAppContext();
            //start a separate thread and start listening to your network object
            ClassAlarm.ProgramAlarms(Context);
            base.OnCreate();
        }
    }
}