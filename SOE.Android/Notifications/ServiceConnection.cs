using Android.Content;
using Android.OS;
using Xamarin.Forms.Internals;

namespace SOE.Droid.Notifications
{
    [Preserve]
    public class ServiceConnection: Java.Lang.Object,IServiceConnection
    {
        private readonly Context AppContext;
        public ServiceConnection(Context AppContext)
        {
            this.AppContext = AppContext;
        }

        public void OnServiceConnected(ComponentName name, IBinder binder)
        {
            NotificationService service = null;
           if (binder is AlarmBinder alarmBinder)
            {
                service = alarmBinder.getService();
                if(service is not null)
                {
                    service.StartCommand();
                }
            }
            this.AppContext.UnbindService(this);
            service?.StopSelf();
            Notification.Cancel(AppContext,Notification.ServiceNotificationId);
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            //:)
        }
    }
}
