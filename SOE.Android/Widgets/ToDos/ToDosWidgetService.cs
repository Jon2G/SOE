using Android.App;
using Android.Content;
using Android.Widget;

namespace SOE.Droid.Widgets.ToDos
{
    [Service(Permission = "android.permission.BIND_REMOTEVIEWS",Exported = false)]
    public class ToDosWidgetService : RemoteViewsService
    {

        public override IRemoteViewsFactory? OnGetViewFactory(Intent? intent)
        {
            return new ToDosRemoteViewsFactory(this.ApplicationContext, intent);
        }

        public override void OnCreate()
        {
            base.OnCreate();
        }
    }
}