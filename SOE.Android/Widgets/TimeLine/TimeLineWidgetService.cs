#nullable enable
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Widget;

namespace SOE.Droid.Widgets.TimeLine
{
    [Preserve]
    [Service(Permission = "android.permission.BIND_REMOTEVIEWS",Exported = false)]
    public class TimeLineWidgetService : RemoteViewsService
    {

        public override IRemoteViewsFactory? OnGetViewFactory(Intent? intent)
        {
            return new TimeLineRemoteViewsFactory(this.ApplicationContext, intent);
        }

        public override void OnCreate()
        {
            base.OnCreate();
        }
    }
}