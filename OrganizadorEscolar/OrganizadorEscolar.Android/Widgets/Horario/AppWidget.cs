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
using OrganizadorEscolar.Models.Horario;

namespace OrganizadorEscolar.Droid.Widgets.Horario
{
    [BroadcastReceiver(Label = "Horario")]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/appwidgetprovider")]
    public class AppWidget : AppWidgetProvider
    {
        public const string TOAST_ACTION = "com.example.android.AppWidgetProvider.TOAST_ACTION";
        public const string EXTRA_ITEM = "com.example.android.AppWidgetProvider.EXTRA_ITEM";


        public AppWidget()
        {
            if (OrganizadorEscolar.Widgets.Horario.WidgetHorario.Instance is null)
            {
                OrganizadorEscolar.Widgets.Horario.WidgetHorario.Init(new WidgetHorario(null));
            }
        }
        public override void OnEnabled(Context context)
        {
            base.OnEnabled(context);
        }
        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            // There may be multiple widgets active, so update all of them
            // update each of the widgets with the remote adapter
            for (int i = 0; i < appWidgetIds.Length; ++i)
            {
                // Here we setup the intent which points to the StackViewService which will
                // provide the views for this collection.
                Intent intent = new Intent(context, Java.Lang.Class.FromType(typeof(ListWidgetService)));
                intent.PutExtra(AppWidgetManager.ExtraAppwidgetId, appWidgetIds[i]);

                // When intents are compared, the extras are ignored, so we need to embed the extras
                // into the data so that the extras will not be ignored.
                intent.SetData(Android.Net.Uri.Parse(intent.ToUri(IntentUriType.Scheme)));

                // Construct the RemoteViews object
                RemoteViews views = new RemoteViews(context.PackageName, Resource.Layout.WidgetHorario);
                views.SetRemoteAdapter(Resource.Id.list_view, intent);

                // The empty view is displayed when the collection has no items. It should be a sibling
                // of the collection view.
                views.SetEmptyView(Resource.Id.list_view, Resource.Id.empty_view);

                // This section makes it possible for items to have individualized behavior.
                // It does this by setting up a pending intent template. Individuals items of a collection
                // cannot set up their own pending intents. Instead, the collection as a whole sets
                // up a pending intent template, and the individual items set a fillInIntent
                // to create unique behavior on an item-by-item basis.
                Intent toastIntent = new Intent(context, Java.Lang.Class.FromType(typeof(AppWidget)));

                // Set the action for the intent.
                // When the user touches a particular view, it will have the effect of
                // broadcasting TOAST_ACTION.
                toastIntent.SetAction(TOAST_ACTION);
                toastIntent.PutExtra(AppWidgetManager.ExtraAppwidgetId, appWidgetIds[i]);

                PendingIntent toastPendingIntent =
                    PendingIntent.GetBroadcast(context, 0, toastIntent, PendingIntentFlags.UpdateCurrent);
                views.SetPendingIntentTemplate(Resource.Id.list_view, toastPendingIntent);


                //Clicks
                views.SetOnClickPendingIntent(Resource.Id.BtnDiaAntes, GetPendingSelfIntent(context, DiaAnterior));
                views.SetOnClickPendingIntent(Resource.Id.BtnDiaManana, GetPendingSelfIntent(context, DiaManana));


                // Instruct the widget manager to update the widget
                appWidgetManager.UpdateAppWidget(appWidgetIds[i], views);

                SetTextViewText(views);


                //  RegisterClicks(context, views, appWidgetIds[i]);

                appWidgetManager.NotifyAppWidgetViewDataChanged(appWidgetIds[i], Resource.Id.widget_item_horario);
            }
            base.OnUpdate(context, appWidgetManager, appWidgetIds);

        }

        private void SetTextViewText(RemoteViews widgetView)
        {
            widgetView.SetTextViewText(Resource.Id.LblNombreDiaActual, OrganizadorEscolar.Widgets.Horario.WidgetHorario.Instance?.Actual.GetNombreDia());
        }

        private const string DiaAnterior = "DiaAnterior";
        private const string DiaManana = "DiaManana";

        public const string EXTRA_LABEL = "TASK_TEXT";

        private PendingIntent GetPendingSelfIntent(Context context, string action)
        {
            // An explicit intent directed at the current class (the "self").
            Intent intent = new Intent(context, Class);
            intent.SetAction(action);
            return PendingIntent.GetBroadcast(context, 0, intent, 0);
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
            ///
            base.OnReceive(context, intent);
            switch (intent.Action)
            {
                case AppWidgetManager.ActionAppwidgetUpdate:

                    break;
                case DiaAnterior:
                    OrganizadorEscolar.Widgets.Horario.WidgetHorario.Instance?.Ayer();
                    UpdateWiget(context);
                    break;
                case DiaManana:
                    OrganizadorEscolar.Widgets.Horario.WidgetHorario.Instance?.Tomorrow();
                    UpdateWiget(context);
                    break;
                default:
                    return;
            }
            UpdateWiget(context);

        }
        private RemoteViews GetRemoteView(Context context)
        {
            return new RemoteViews(context.PackageName, Resource.Layout.WidgetHorario);
        }
        private void UpdateWiget(Context context)
        {
            AppWidgetManager appWidgetManager = AppWidgetManager.GetInstance(context);

            int[] appWidgetIds = appWidgetManager.GetAppWidgetIds(new ComponentName(context, Java.Lang.Class.FromType(typeof(AppWidget))));

            RemoteViews views = GetRemoteView(context);
            SetTextViewText(views);
            for (int i = 0; i < appWidgetIds.Length; i++)
            {
                appWidgetManager.UpdateAppWidget(appWidgetIds[i], views);
            }
            appWidgetManager.NotifyAppWidgetViewDataChanged(appWidgetIds, Resource.Id.list_view);
        }

    }
}
