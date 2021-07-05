using Android.App;
using Android.Content;
using Android.OS;
using Kit.Droid.Services;
using SOE.Droid.Notifications;

[assembly: Xamarin.Forms.Dependency(typeof(NotificationService))]
namespace SOE.Droid.Notifications
{
    [Service(Enabled = true, Exported = true)]
    [IntentFilter(new[] { ".NotificationService" })]
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
            ClassAlarm.ProgramAlarms(Context);
            ToDoAlarm.ProgramAlarms(Context);
            base.OnCreate();
        }
    }
}