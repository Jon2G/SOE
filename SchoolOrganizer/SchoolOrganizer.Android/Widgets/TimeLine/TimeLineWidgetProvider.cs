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
using Android.Appwidget;
using Java.Lang;
using SchoolOrganizer.Data;
using SchoolOrganizer.Droid.Widgets.RemoteViewsServices;
using String = System.String;

namespace SchoolOrganizer.Droid.Widgets.TimeLine
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new string[] { AppWidgetManager.ActionAppwidgetUpdate,TimeLineWidget.DAY_CHANGED })]
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
            if (AppData.Instance is null)
            {
                AppData.Init();
            }
            base.OnEnabled(context);
        }

        public override void OnReceive(Context context, Intent intent)
        {
            AppWidgetManager mgr = AppWidgetManager.GetInstance(context);
            String IntentAction = intent.Action;
            int appWidgetId = intent.GetIntExtra(AppWidgetManager.ExtraAppwidgetId, 0);
            
            switch (IntentAction)
            {
                case TimeLineWidget.TOAST_ACTION:
                    int viewIndex = intent.GetIntExtra(TimeLineWidget.EXTRA_ITEM, 0);
                    Toast.MakeText(context, "Touched view " + viewIndex, ToastLength.Short).Show();
                    break;
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

            }

            base.OnReceive(context, intent);
        }


        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
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
                Intent toastIntent = new Intent(context, typeof(TimeLineWidgetProvider));
                toastIntent.SetAction(TimeLineWidget.TOAST_ACTION);
                toastIntent.PutExtra(AppWidgetManager.ExtraAppwidgetId, appWidgetIds[i]);
                intent.SetData(Android.Net.Uri.Parse(intent.ToUri(Android.Content.IntentUriType.Scheme)));
                PendingIntent toastPendingIntent = PendingIntent.GetBroadcast(context, 0, toastIntent,
                    PendingIntentFlags.UpdateCurrent);
                rv.SetPendingIntentTemplate(Resource.Id.stack_view, toastPendingIntent);


                Intent fowardIntent = new Intent(context, typeof(TimeLineWidgetProvider));
                fowardIntent.SetAction(TimeLineWidget.FOWARD_ACTION);
                fowardIntent.PutExtra(AppWidgetManager.ExtraAppwidgetId, appWidgetIds[i]);

                PendingIntent fowardPendingIntent = PendingIntent.GetBroadcast(context, 0, fowardIntent, PendingIntentFlags.UpdateCurrent);
                rv.SetOnClickPendingIntent(Resource.Id.widget_timetable_Btntomorrow, fowardPendingIntent);


                Intent backwardIntent = new Intent(context, typeof(TimeLineWidgetProvider));
                backwardIntent.SetAction(TimeLineWidget.BACKWARD_ACTION);
                backwardIntent.PutExtra(AppWidgetManager.ExtraAppwidgetId, appWidgetIds[i]);

                PendingIntent backwardPendingIntent = PendingIntent.GetBroadcast(context, 0, backwardIntent,
                    PendingIntentFlags.UpdateCurrent);
                rv.SetOnClickPendingIntent(Resource.Id.widget_timetable_Btnyesterday, backwardPendingIntent);

                appWidgetManager.UpdateAppWidget(appWidgetIds[i], rv);
            }

            base.OnUpdate(context, appWidgetManager, appWidgetIds);
        }

    }
}