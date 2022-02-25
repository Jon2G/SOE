using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Widget;
using SOE.Droid.Activities;
using SOE.Models.TodoModels;
using SOE.Widgets;
using SQLitePCL;
using String = System.String;

namespace SOE.Droid.Widgets.ToDos
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new string[] { AppWidgetManager.ActionAppwidgetUpdate,
        AppWidgetManager.ActionAppwidgetEnabled, AppWidgetManager.ActionAppwidgetDeleted,
        AppWidgetManager.ActionAppwidgetDisabled,ToDosWidget.ITEM_CLICK})]
    [MetaData(AppWidgetManager.MetaDataAppwidgetProvider, Resource = "@xml/widget_todos_appwidgetprovider")]
    [Preserve]
    public class ToDosWidgetProvider : AppWidgetProvider
    {
        public override void OnDeleted(Context context, int[] appWidgetIds)
        {
            base.OnDeleted(context, appWidgetIds);
        }

        public override void OnDisabled(Context context)
        {
            base.OnDisabled(context);
        }

        public override void OnEnabled(Context context)
        {
            base.OnEnabled(context);
            //if (AppData.Instance is null)
            //{
            //    AppData.Init();
            //}

        }
        public override void OnAppWidgetOptionsChanged(Context context, AppWidgetManager appWidgetManager, int appWidgetId, Bundle newOptions)
        {
            base.OnAppWidgetOptionsChanged(context, appWidgetManager, appWidgetId, newOptions);
        }

        public override void OnReceive(Context context, Intent intent)
        {
            base.OnReceive(context, intent);
            AppWidgetManager mgr = AppWidgetManager.GetInstance(context);
            String IntentAction = intent.Action;
            int[]? appWidgetIds = intent.GetIntArrayExtra(AppWidgetManager.ExtraAppwidgetIds);
            if (appWidgetIds is null || appWidgetIds.Length <= 0)
            {
                appWidgetIds = new[] { intent.GetIntExtra(AppWidgetManager.ExtraAppwidgetId, 0) };
            }
            foreach (int appWidgetId in appWidgetIds)
            {
                switch (IntentAction)
                {
                    case AppWidgetManager.ActionAppwidgetUpdate:
                        ToDosWidget.Refresh(appWidgetId);
                        OnUpdate(context, mgr, new int[] { appWidgetId });
                        break;
                    case AppWidgetManager.ActionAppwidgetOptionsChanged:
                    case AppWidgetManager.ActionAppwidgetEnabled:
                        ToDosWidget.Refresh(appWidgetId);
                        OnUpdate(context, mgr, new int[] { appWidgetId });
                        break;
                    case AppWidgetManager.ActionAppwidgetDeleted:
                        ToDosWidget.Unload(appWidgetId);
                        break;
                    case ToDosWidget.ITEM_CLICK:
                        Intent OpenClassTimeDetails = new Intent(context, typeof(MainActivity));
                        int itemPosition = intent.GetIntExtra(ToDosWidget.EXTRA_ITEM, 0);
                        ToDo todoItem = ToDosWidget.GetItemAt(appWidgetId, itemPosition);
                        OpenClassTimeDetails.PutExtra(nameof(ToDo.DocumentId), todoItem.DocumentId);
                        //OpenClassTimeDetails.SetFlags(ActivityFlags.NewTask);
                        OpenClassTimeDetails.SetAction(IntentAction);
                        OpenClassTimeDetails.SetFlags(ActivityFlags.SingleTop | ActivityFlags.BroughtToFront |
                                                      ActivityFlags.NewTask);
                        context.StartActivity(OpenClassTimeDetails);
                        break;
                    default:
                        OnUpdate(context, mgr, new[] { appWidgetId });
                        break;
                }
            }

        }

        private PendingIntent GetIntent(Context context, int widgetId, string action)
        {
            Intent fowardIntent = new Intent(context, this.Class);
            fowardIntent.SetAction(action);
            fowardIntent.PutExtra(AppWidgetManager.ExtraAppwidgetId, widgetId);
            return PendingIntent.GetBroadcast(context, 0, fowardIntent, PendingIntentFlags.UpdateCurrent);
        }
        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            base.OnUpdate(context, appWidgetManager, appWidgetIds);
            // update each of the widgets with the remote adapter
            for (int i = 0; i < appWidgetIds.Length; ++i)
            {
                // Here we setup the intent which points to the StackViewService which will
                // provide the views for this collection.
                Intent intent = new Intent(context, typeof(ToDosWidgetService));
                intent.PutExtra(AppWidgetManager.ExtraAppwidgetId, appWidgetIds[i]);
                // When intents are compared, the extras are ignored, so we need to embed the extras
                // into the data so that the extras will not be ignored.
                intent.SetData(Android.Net.Uri.Parse(intent.ToUri(Android.Content.IntentUriType.Scheme)));

                RemoteViews rv = new RemoteViews(context.PackageName, Resource.Layout.layout_widget_todos);

                rv.SetRemoteAdapter(Resource.Id.stack_view_todos, intent);
                // The empty view is displayed when the collection has no items. It should be a sibling
                // of the collection view.
                rv.SetEmptyView(Resource.Id.stack_view_todos, Resource.Id.empty_view_todos);
                // Here we setup the a pending intent template. Individuals items of a collection
                // cannot setup their own pending intents, instead, the collection as a whole can
                // setup a pending intent template, and the individual items can set a fillInIntent
                // to create unique before on an item to item basis.
                rv.SetPendingIntentTemplate(Resource.Id.stack_view_todos,
                    GetIntent(context, appWidgetIds[i], ToDosWidget.ITEM_CLICK));
                //update stack list
                appWidgetManager.NotifyAppWidgetViewDataChanged(appWidgetIds[i], Resource.Id.stack_view_todos);
                //is this layout update wrong???
                appWidgetManager.NotifyAppWidgetViewDataChanged(appWidgetIds[i], rv.LayoutId);

                appWidgetManager.UpdateAppWidget(appWidgetIds[i], rv);
            }
        }

    }
}