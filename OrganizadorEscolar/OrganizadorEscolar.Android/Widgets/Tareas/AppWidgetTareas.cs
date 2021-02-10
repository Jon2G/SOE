using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace OrganizadorEscolar.Droid.Widgets.Tareas
{
    [BroadcastReceiver(Label = "Tareas")]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/appwidgetprovidertareas")]
    public class AppWidgetTareas : AppWidgetProvider
    {
        public const string TOAST_ACTION = "com.example.android.AppWidgetProvider.TOAST_ACTION";
        public const string EXTRA_ITEM = "com.example.android.AppWidgetProvider.EXTRA_ITEM";


        public AppWidgetTareas()
        {
            if (OrganizadorEscolar.Widgets.Tareas.WidgetTareas.Instance is null)
            {
                OrganizadorEscolar.Widgets.Tareas.WidgetTareas.Init(new WidgetTareas(null));
            }
        }
        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            // There may be multiple widgets active, so update all of them
            // update each of the widgets with the remote adapter
            for (int i = 0; i < appWidgetIds.Length; ++i)
            {
                // Here we setup the intent which points to the StackViewService which will
                // provide the views for this collection.
                Intent intent = new Intent(context, Java.Lang.Class.FromType(typeof(ListWidgetServiceTareas)));
                intent.PutExtra(AppWidgetManager.ExtraAppwidgetId, appWidgetIds[i]);

                // When intents are compared, the extras are ignored, so we need to embed the extras
                // into the data so that the extras will not be ignored.
                intent.SetData(Android.Net.Uri.Parse(intent.ToUri(IntentUriType.Scheme)));

                // Construct the RemoteViews object
                RemoteViews views = new RemoteViews(context.PackageName, Resource.Layout.WidgetTareas);
                views.SetRemoteAdapter(Resource.Id.list_view_tareas, intent);

                // The empty view is displayed when the collection has no items. It should be a sibling
                // of the collection view.
                views.SetEmptyView(Resource.Id.list_view_tareas, Resource.Id.empty_view_tareas);

                // This section makes it possible for items to have individualized behavior.
                // It does this by setting up a pending intent template. Individuals items of a collection
                // cannot set up their own pending intents. Instead, the collection as a whole sets
                // up a pending intent template, and the individual items set a fillInIntent
                // to create unique behavior on an item-by-item basis.
                Intent toastIntent = new Intent(context, Java.Lang.Class.FromType(typeof(AppWidgetTareas)));

                // Set the action for the intent.
                // When the user touches a particular view, it will have the effect of
                // broadcasting TOAST_ACTION.
                toastIntent.SetAction(TOAST_ACTION);
                toastIntent.PutExtra(AppWidgetManager.ExtraAppwidgetId, appWidgetIds[i]);

                PendingIntent toastPendingIntent =
                    PendingIntent.GetBroadcast(context, 0, toastIntent, PendingIntentFlags.UpdateCurrent);
                views.SetPendingIntentTemplate(Resource.Id.list_view_tareas, toastPendingIntent);


                //Clicks
                views.SetOnClickPendingIntent(Resource.Id.BtnAgregarTarea, GetPendingSelfIntent(context, AgregarTarea));


                // Instruct the widget manager to update the widget
                appWidgetManager.UpdateAppWidget(appWidgetIds[i], views);



                //  RegisterClicks(context, views, appWidgetIds[i]);

                appWidgetManager.NotifyAppWidgetViewDataChanged(appWidgetIds[i], Resource.Id.widget_item_tareas);
            }
            base.OnUpdate(context, appWidgetManager, appWidgetIds);

        }

        private const string AgregarTarea = "AgregarTarea";

        public const string EXTRA_LABEL = "TASK_TEXT";

        private PendingIntent GetPendingSelfIntent(Context context, string action)
        {
            // An explicit intent directed at the current class (the "self").
            Intent intent = new Intent(context, Class);
            intent.SetAction(action);
            return PendingIntent.GetBroadcast(context, 1, intent, 0);
        }
        /// <summary>
        /// Called when the BroadcastReceiver receives an Intent broadcast.
        /// Checks to see whether the intent's action is TOAST_ACTION. If it is, the app widget
        /// displays a Toast message for the current item.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="intent"></param>
        public override void OnReceive(Context context, Intent intent)
        {
            ///
            base.OnReceive(context, intent);
            if (intent.Action.Equals(AppWidgetManager.ActionAppwidgetUpdate))
            {
                UpdateWiget(context);
                return;
            }
            if (intent.Action.Equals(AppWidgetManager.ActionAppwidgetDeleted))
            {
                return;
            }
            else
            if (intent.Action.Equals(TOAST_ACTION))
            {
                //  int appWidgetId = intent.GetIntExtra(AppWidgetManager.ExtraAppwidgetId,AppWidgetManager.InvalidAppwidgetId);
                int viewIndex = intent.GetIntExtra(EXTRA_ITEM, -1);
                string NombreMateria = intent.GetStringExtra(EXTRA_LABEL);
                ///
                Intent goToDetails = new Intent(context, Java.Lang.Class.FromType(typeof(MainActivity)));
                goToDetails.PutExtra(EXTRA_ITEM, viewIndex);
                goToDetails.PutExtra(EXTRA_LABEL, NombreMateria);
                goToDetails.SetFlags(ActivityFlags.NewTask);
                goToDetails.SetData(Android.Net.Uri.Parse(goToDetails.ToUri(IntentUriType.Scheme)));
                context.StartActivity(goToDetails);
                ///
                Toast.MakeText(context, "Item" + ++viewIndex + " selected", ToastLength.Short).Show();
            }
            else if (intent.Action.Equals(AgregarTarea))
            {

            }
            else
            {
                base.OnReceive(context, intent);
                return;
            }
         //   UpdateWiget(context);

        }
        private RemoteViews GetRemoteView(Context context)
        {
            return new RemoteViews(context.PackageName, Resource.Layout.WidgetTareas);
        }
        private void UpdateWiget(Context context)
        {
            AppWidgetManager appWidgetManager = AppWidgetManager.GetInstance(context);

            int[] appWidgetIds = appWidgetManager.GetAppWidgetIds(new ComponentName(context, Java.Lang.Class.FromType(typeof(AppWidgetTareas))));

            RemoteViews views = GetRemoteView(context);

            for (int i = 0; i < appWidgetIds.Length; i++)
            {
                appWidgetManager.UpdateAppWidget(appWidgetIds[i], views);
            }
            appWidgetManager.NotifyAppWidgetViewDataChanged(appWidgetIds, Resource.Id.list_view_tareas);
            OnUpdate(context, appWidgetManager, appWidgetIds);
        }

    }
}
