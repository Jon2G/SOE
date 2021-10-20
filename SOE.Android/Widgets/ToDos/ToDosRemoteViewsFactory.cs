using System.Collections.Generic;
using System.Linq;
using Android.Appwidget;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using Kit;
using Kit.Droid;
using SOE.Data;
using SOE.Models.Scheduler;
using SOE.Models.TodoModels;
using SOE.Widgets;

namespace SOE.Droid.Widgets.ToDos
{

    public class ToDosRemoteViewsFactory : Java.Lang.Object, RemoteViewsService.IRemoteViewsFactory
    {
        private Context mContext;
        private int mAppWidgetId;
        private List<ToDo> Todos;

        public ToDosRemoteViewsFactory() { }
        public ToDosRemoteViewsFactory(Context context, Intent intent)
        {
            mContext = context;
            mAppWidgetId = intent.GetIntExtra(AppWidgetManager.ExtraAppwidgetId, AppWidgetManager.InvalidAppwidgetId);
        }


        public void OnCreate()
        {
            // In onCreate() you setup any connections / cursors to your data source. Heavy lifting,
            // for example downloading or creating content etc, should be deferred to onDataSetChanged()
            // or getViewAt(). Taking more than 20 seconds in this call will result in an ANR.
            if (AppData.Instance is null)
            {
                AppData.Init();
            }
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
            Todos?.Clear();
        }

        public int Count
        {
            get
            {
                return Todos.Count;
            }
        }

        public RemoteViews GetViewAt(int position)
        {
            // position will always range from 0 to getCount() - 1.
            // We construct a remote views item based on our widget item xml file, and set the
            // text based on the position.
            ToDo todo = this.Todos.ElementAt(position);
            RemoteViews rv = new RemoteViews(mContext.PackageName, Resource.Layout.widget_todos_task);

            rv.SetTextViewText(Resource.Id.widget_todos_SubjectName, todo.Subject.Name);
            rv.SetTextViewText(Resource.Id.widget_todos_SubjectGroup, todo.Subject.Group);
            rv.SetTextViewText(Resource.Id.widget_todos_TaskName, todo.Title);
            rv.SetTextViewText(Resource.Id.widget_todos_TaskTime, todo.FormattedTime);
            rv.SetTextViewText(Resource.Id.widget_todos_TaskDate, todo.FormattedDate);
            rv.SetTextViewText(Resource.Id.widget_todos_TaskEmoji, ToDosWidget.GetEmoji(todo));
            rv.SetInt(Resource.Id.widget_todos_SubjectName, "setBackgroundColor", Color.ParseColor(mContext.IsDarkMode() ? todo.Subject.ColorDark : todo.Subject.Color));
            rv.SetInt(Resource.Id.widget_todos_StatusColor, "setBackgroundColor", Color.ParseColor(ToDosWidget.GetColor(todo)));


            // Next, we set a fill-intent which will be used to fill-in the pending intent template
            // which is set on the collection view in StackWidgetProvider.
            Bundle extras = new Bundle();
            extras.PutInt(TimeLineWidget.EXTRA_ITEM, position);
            Intent fillInIntent = new Intent();
            fillInIntent.PutExtras(extras);
            rv.SetOnClickFillInIntent(Resource.Id.widget_todo_item, fillInIntent);
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
            this.Todos = ToDosWidget.GetTasks(this.mAppWidgetId);
        }



    }
}