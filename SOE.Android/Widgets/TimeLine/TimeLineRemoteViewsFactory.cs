using Android.Appwidget;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Kit;
using SOE.Models.Scheduler;
using SOE.Widgets;
using System.Collections.Generic;
using System.Linq;

namespace SOE.Droid.Widgets.TimeLine
{

    [Preserve]
    public class TimeLineRemoteViewsFactory : Java.Lang.Object, RemoteViewsService.IRemoteViewsFactory
    {
        private Context mContext;
        private int mAppWidgetId;
        private List<ClassSquare> Schedule;

        public TimeLineRemoteViewsFactory() { }
        public TimeLineRemoteViewsFactory(Context context, Intent intent)
        {
            mContext = context;
            mAppWidgetId = intent.GetIntExtra(AppWidgetManager.ExtraAppwidgetId, AppWidgetManager.InvalidAppwidgetId);
        }


        public void OnCreate()
        {
            // In onCreate() you setup any connections / cursors to your data source. Heavy lifting,
            // for example downloading or creating content etc, should be deferred to onDataSetChanged()
            // or getViewAt(). Taking more than 20 seconds in this call will result in an ANR.

            // We sleep for 3 seconds here to show how the empty view appears in the interim.
            // The empty view is set in the StackWidgetProvider and should be a sibling of the
            // collection view.
            /*try {
                Thread.sleep(3000);
            } catch (InterruptedException e) {
                e.printStackTrace();
            }*/
        }
        public void OnDestroy()
        {
            // In onDestroy() you should tear down anything that was setup for your data source,
            // eg. cursors, connections, etc.
            TimeLineWidget.Unload(this.mAppWidgetId);
            Schedule?.Clear();
        }

        public int Count
        {
            get
            {
                return Schedule.Count;
            }
        }

        public RemoteViews GetViewAt(int position)
        {
            // position will always range from 0 to getCount() - 1.
            // We construct a remote views item based on our widget item xml file, and set the
            // text based on the position.
            var subject = this.Schedule.ElementAt(position);
            RemoteViews rv = new RemoteViews(mContext.PackageName, Resource.Layout.widget_timetable_subject);

            rv.SetTextViewText(Resource.Id.widget_timetable_TextViewSubjectName, subject.Subject.Name);
            rv.SetTextViewText(Resource.Id.widget_timetable_TextViewSubjectTime, subject.FormattedTime);
            rv.SetTextViewText(Resource.Id.widget_timetable_TextViewSalon, subject.Subject.Group.Name);
            rv.SetInt(Resource.Id.widget_timetable_SubjectColor, "setBackgroundColor", Color.ParseColor(subject.Subject.Color));


            // Next, we set a fill-intent which will be used to fill-in the pending intent template
            // which is set on the collection view in StackWidgetProvider.
            Bundle extras = new Bundle();
            extras.PutInt(TimeLineWidget.EXTRA_ITEM, position);
            Intent fillInIntent = new Intent();
            fillInIntent.PutExtras(extras);
            rv.SetOnClickFillInIntent(Resource.Id.widget_timetable_item, fillInIntent);
            // You can do heaving lifting in here, synchronously. For example, if you need to
            // process an image, fetch something from the network, etc., it is ok to do it here,
            // synchronously. A loading view will show up in lieu of the actual contents in the
            // interim.
            Log.Logger.Debug("Loading view " + position);
            //Thread.sleep(500);

            // Return the remote views object.
            return rv;
        }


        bool RemoteViewsService.IRemoteViewsFactory.HasStableIds => true;
        // You can create a custom loading view (for instance when getViewAt() is slow.) If you
        // return null here, you will get the default loading view.
        public RemoteViews LoadingView => null;

        public int ViewTypeCount => 1;


        public long GetItemId(int position)
        {
            return position;
        }

        public void OnDataSetChanged()
        {
            // This is triggered when you call AppWidgetManager notifyAppWidgetViewDataChanged
            // on the collection view corresponding to this factory. You can do heaving lifting in
            // here, synchronously. For example, if you need to process an image, fetch something
            // from the network, etc., it is ok to do it here, synchronously. The widget will remain
            // in its current state while work is being done here, so you don't need to worry about
            // locking up the widget.
            this.Schedule = TimeLineWidget.GetTimeLine(this.mAppWidgetId).GetAwaiter().GetResult();
        }



    }
}