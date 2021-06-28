using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Design;
using System.Linq;
using System.Text;
using Android.Appwidget;
using Google.Android.Material.Internal;
using Java.Lang;
using SchoolOrganizer.Data;
using SchoolOrganizer.Droid.Activities;
using SchoolOrganizer.Models.Scheduler;
using SchoolOrganizer.Widgets;
using String = System.String;

namespace SchoolOrganizer.Droid.Widgets.TimeLine
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new string[] { AppWidgetManager.ActionAppwidgetUpdate,
        AppWidgetManager.ActionAppwidgetEnabled, AppWidgetManager.ActionAppwidgetDeleted,
        AppWidgetManager.ActionAppwidgetDisabled,TimeLineWidget.ITEM_CLICK,TimeLineWidget.DAY_CLICK })]
    [MetaData(AppWidgetManager.MetaDataAppwidgetProvider, Resource = "@xml/widget_timetable_appwidgetprovider")]
    public class TimeLineWidgetProvider : AppWidgetProvider
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
            int appWidgetId = intent.GetIntExtra(AppWidgetManager.ExtraAppwidgetId, 0);
            if (appWidgetId == 0) { return; }
            switch (IntentAction)
            {
                case TimeLineWidget.BACKWARD_ACTION:
                    TimeLineWidget.Yesterday(appWidgetId);
                    Intent updateIntentb = new Intent(intent.Action);
                    context.SendBroadcast(updateIntentb);
                    OnUpdate(context, mgr, new int[] { appWidgetId });
                    mgr.NotifyAppWidgetViewDataChanged(appWidgetId, Resource.Id.stack_view);
                    break;
                case TimeLineWidget.FOWARD_ACTION:
                    TimeLineWidget.Tomorrow(appWidgetId);
                    Intent updateIntentf = new Intent(intent.Action);
                    context.SendBroadcast(updateIntentf);
                    OnUpdate(context, mgr, new int[] { appWidgetId });
                    mgr.NotifyAppWidgetViewDataChanged(appWidgetId, Resource.Id.stack_view);
                    break;
                case AppWidgetManager.ActionAppwidgetOptionsChanged:
                case AppWidgetManager.ActionAppwidgetEnabled:
                case AppWidgetManager.ActionAppwidgetUpdate:
                    TimeLineWidget.Today(appWidgetId);
                    OnUpdate(context, mgr, new int[] { appWidgetId });
                    break;
                case AppWidgetManager.ActionAppwidgetDeleted:
                    TimeLineWidget.Unload(appWidgetId);
                    break;
                case TimeLineWidget.ITEM_CLICK:
                    Intent OpenClassTimeDetails = new Intent(context, typeof(SplashActivity));
                    int itemPosition = intent.GetIntExtra(TimeLineWidget.EXTRA_ITEM, 0);
                    ClassSquare classItem = TimeLineWidget.GetItemAt(appWidgetId, itemPosition);
                    OpenClassTimeDetails.PutExtra(nameof(ClassTime.Group), classItem.Subject.Group);
                    OpenClassTimeDetails.PutExtra(nameof(ClassTime.Begin), classItem.Begin.Ticks);
                    OpenClassTimeDetails.PutExtra(nameof(ClassTime.Day), (int)classItem.Day);
                    //OpenClassTimeDetails.SetFlags(ActivityFlags.NewTask);
                    OpenClassTimeDetails.SetAction(IntentAction);
                    OpenClassTimeDetails.SetFlags(ActivityFlags.SingleTop | ActivityFlags.BroughtToFront | ActivityFlags.NewTask);
                    context.StartActivity(OpenClassTimeDetails);
                    break;
                case TimeLineWidget.DAY_CLICK:
                    Intent OpenDayDetails = new Intent(context, typeof(SplashActivity));
                    OpenDayDetails.SetAction(IntentAction);
                    OpenDayDetails.PutExtra(nameof(ClassTime.Day), (int)TimeLineWidget.GetDay(appWidgetId).DayOfWeek);
                    OpenDayDetails.SetFlags(ActivityFlags.SingleTop | ActivityFlags.BroughtToFront | ActivityFlags.NewTask);
                    context.StartActivity(OpenDayDetails);
                    break;
                default:
                    OnUpdate(context, mgr,new []{ appWidgetId } );
                    break;
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
                Intent intent = new Intent(context, typeof(TimeLineWidgetService));
                intent.PutExtra(AppWidgetManager.ExtraAppwidgetId, appWidgetIds[i]);
                // When intents are compared, the extras are ignored, so we need to embed the extras
                // into the data so that the extras will not be ignored.
                intent.SetData(Android.Net.Uri.Parse(intent.ToUri(Android.Content.IntentUriType.Scheme)));

                RemoteViews rv = new RemoteViews(context.PackageName, Resource.Layout.layout_widget_timetable);
                rv.SetTextViewText(Resource.Id.widget_timetable_TextViewCurrent_day, TimeLineWidget.GetDay(appWidgetIds[i]).Name);

                rv.SetRemoteAdapter(Resource.Id.stack_view, intent);
                // The empty view is displayed when the collection has no items. It should be a sibling
                // of the collection view.
                rv.SetEmptyView(Resource.Id.stack_view, Resource.Id.empty_view);
                // Here we setup the a pending intent template. Individuals items of a collection
                // cannot setup their own pending intents, instead, the collection as a whole can
                // setup a pending intent template, and the individual items can set a fillInIntent
                // to create unique before on an item to item basis.
                rv.SetPendingIntentTemplate(Resource.Id.stack_view,
                    GetIntent(context, appWidgetIds[i], TimeLineWidget.ITEM_CLICK));

                rv.SetOnClickPendingIntent(Resource.Id.widget_timetable_Btntomorrow,
                    GetIntent(context, appWidgetIds[i], TimeLineWidget.FOWARD_ACTION));

                rv.SetOnClickPendingIntent(Resource.Id.widget_timetable_Btnyesterday,
                    GetIntent(context, appWidgetIds[i], TimeLineWidget.BACKWARD_ACTION));

                rv.SetOnClickPendingIntent(Resource.Id.widget_timetable_TextViewCurrent_day,
                    GetIntent(context, appWidgetIds[i], TimeLineWidget.DAY_CLICK));

                //update stack list
                appWidgetManager.NotifyAppWidgetViewDataChanged(appWidgetIds[i], Resource.Id.stack_view);
                //is this layout update wrong???
                appWidgetManager.NotifyAppWidgetViewDataChanged(appWidgetIds[i], rv.LayoutId);

                appWidgetManager.UpdateAppWidget(appWidgetIds[i], rv);
            }
        }

    }
}