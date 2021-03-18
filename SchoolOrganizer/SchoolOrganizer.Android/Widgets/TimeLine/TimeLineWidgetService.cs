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

namespace SchoolOrganizer.Droid.Widgets.RemoteViewsServices
{
    [Service(Permission = "android.permission.BIND_REMOTEVIEWS",Exported = false)]
    public class TimeLineWidgetService : RemoteViewsService
    {

        public override IRemoteViewsFactory? OnGetViewFactory(Intent? intent)
        {
            return new TimeLineRemoteViewsFactory(this.ApplicationContext, intent);
        }
    }
}