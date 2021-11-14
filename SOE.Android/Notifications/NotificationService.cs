using Android.App;
using Android.Content;
using Android.OS;
using Kit.Droid.Services;
using SOE.Droid.Notifications;
using SOE.Droid.Notifications.Alarms;
using SOE.Notifications;
using Xamarin.Forms.Internals;

namespace SOE.Droid.Notifications
{
    [Service(Enabled = true, Exported = true)]
    [IntentFilter(new[] { ".NotificationService" })]
    [Preserve]
    public class NotificationService : Service
    {
        public override ComponentName? StartForegroundService(Intent? service)
        {
            return base.StartForegroundService(service);
        }

        public override IBinder? OnBind(Intent? intent) => null;
        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            base.OnStartCommand(intent, flags, startId);
            return StartCommandResult.Sticky;
        }
        public override void OnCreate()
        {
            this.ScheduleAll();
            base.OnCreate();
        }

        public void ScheduleAll()
        {
            try
            {
                ClassAlarm alarm = new();
                alarm.ScheduleAll();
                ToDoAlarm todo = new();
                todo.ScheduleAll();
            }
            catch (System.Exception ex)
            {
                Kit.Log.Logger?.Error(ex, "ScheduleAll");
            }

        }
    }
}