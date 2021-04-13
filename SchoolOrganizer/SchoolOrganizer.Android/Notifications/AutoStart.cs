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
        private Alarm Alarm;
        public AutoStart()
        {
        }
        public override void OnReceive(Context context, Intent intent)
        {
            //if (intent.Action == Intent.ActionBootCompleted)
            //{
                this.Alarm = new Alarm();
                this.Alarm.Start(context);
                //Intent startServiceIntent = new Intent(context, typeof(NotificationService));
                //context.StartService(startServiceIntent);
            //}
        }
    }
}