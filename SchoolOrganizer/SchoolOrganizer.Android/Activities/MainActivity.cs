using System;
using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using ImageCircle.Forms.Plugin.Droid;
using Plugin.Fingerprint;
using Plugin.Media;
using SchoolOrganizer.Droid.Widgets.TimeLine;
using SchoolOrganizer.Models.Scheduler;
using SchoolOrganizer.Widgets;
using Xamarin.Forms;

namespace SchoolOrganizer.Droid.Activities
{
    [Activity(Label = "OrganizadorEscolar", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : Kit.Droid.Services.MainActivity// global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnRestart()
        {
            base.OnRestart();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            CrossFingerprint.SetCurrentActivityResolver(() => this);
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(savedInstanceState);
            await CrossMedia.Current.Initialize();//
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            ImageCircleRenderer.Init();
            UserDialogs.Init(this);
            Rg.Plugins.Popup.Popup.Init(this); // :)
            //OrganizadorEscolar.Widgets.Horario.WidgetHorario.Init(new Widgets.Horario.WidgetHorario(this));
            //OrganizadorEscolar.Widgets.Tareas.WidgetTareas.Init(new Widgets.Tareas.WidgetTareas(this));
            Plugin.InputKit.Platforms.Droid.Config.Init(this, savedInstanceState);
            Kit.Droid.Tools.Init(this, savedInstanceState);
            LoadApplication(new SchoolOrganizer.App());
            if(this.Intent!=null)
                OnNewIntent(this.Intent);

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override void StartIntentSender(IntentSender? intent, Intent? fillInIntent, ActivityFlags flagsMask, ActivityFlags flagsValues,
            int extraFlags, Bundle? options)
        {
            base.StartIntentSender(intent, fillInIntent, flagsMask, flagsValues, extraFlags, options);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            WidgetPendingAction pendingAction = null;
            switch (intent?.Action)
            {
                case TimeLineWidget.ITEM_CLICK:
                    long ticks = intent.GetLongExtra(nameof(ClassSquare.Begin), 0);
                    string group = intent.GetStringExtra(nameof(ClassSquare.Group));
                    DayOfWeek dayOfWeek = (DayOfWeek)intent.GetIntExtra(nameof(ClassSquare.Day), 1);
                    pendingAction = new WidgetPendingAction(intent.Action, new DateTime(ticks), group, dayOfWeek);
                    break;
                case TimeLineWidget.DAY_CLICK:
                    DayOfWeek dayclicked = (DayOfWeek)intent.GetIntExtra(nameof(ClassSquare.Day), 1);
                    pendingAction = new WidgetPendingAction(intent.Action, dayclicked);
                    break;
            }
            if (pendingAction != null)
                AppShell.ResponseTo(pendingAction);
        }
    }
}