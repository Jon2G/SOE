using Android.App;
using Android.Content;
using Android.Widget;

namespace SchoolOrganizer.Droid.Widgets.TimeLine
{
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