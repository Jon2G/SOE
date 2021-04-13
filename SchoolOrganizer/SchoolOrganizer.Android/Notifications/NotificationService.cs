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
using Java.Security;

namespace SchoolOrganizer.Droid.Notifications
{
    [Service(Enabled = true, Process = ":NotificationService")]
    public class NotificationService : Service
    {

        private readonly Alarm Alarm;
        public NotificationService()
        {
            this.Alarm = new Alarm();
        }
        public override void OnCreate()
        {
            base.OnCreate();
        }
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Alarm.Start(this);
            return StartCommandResult.Sticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

    }
}