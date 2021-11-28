using Android.OS;
using Xamarin.Forms.Internals;
using Java.Lang.Ref;
namespace SOE.Droid.Notifications
{
    [Preserve]
    public class AlarmBinder:Binder
    {
        private WeakReference WeakService;

        //Inject service instance to weak reference
        public void OnBind(NotificationService service)
        {
            this.WeakService = new(service);
        }
        public NotificationService getService()
        {
            return WeakService == null ? null : (NotificationService)WeakService.Get();
        }

        public AlarmBinder()
        {
        }
    }
}
