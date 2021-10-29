using Android.App;
using Android.Content;
using Android.OS;
using Kit.Droid.Services;
using SOE.Droid.Notifications;
using SOE.Droid.Notifications.Alarms;
using Xamarin.Forms.Internals;

[assembly: Xamarin.Forms.Dependency(typeof(NotificationService))]
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
            var Context = MainActivity.GetAppContext();
            //start a separate thread and start listening to your network object
            ClassAlarm alarm = new ClassAlarm();
            alarm.ScheduleAlll();

            ToDoAlarm todo = new ToDoAlarm();
            alarm.ScheduleAlll();
            
            base.OnCreate();
        }
    }
}