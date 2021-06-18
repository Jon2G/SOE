using System;
using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Kit;
using Kit.Droid;
using PanCardView.Droid;
using Plugin.CurrentActivity;
using Plugin.Fingerprint;
using Plugin.Media;
using Plugin.Media.Abstractions;
using SchoolOrganizer.Droid.Notifications;
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

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            if (!this.IsServiceRunning(typeof(NotificationService)))
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.Q)
                {
                    StartService(new Intent(this, typeof(NotificationService)));

                }
                else
                {
                    StartForegroundService(new Intent(this, typeof(NotificationService)));
                    StartService(new Intent(this, typeof(NotificationService)));
                }
            }


            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            await CrossMedia.Current.Initialize();//
            base.OnCreate(savedInstanceState);

          //  SetTheme(Android.Resource.Style.DarkTheme);
            

            Kit.Droid.Tools.Init(this, savedInstanceState);
            CrossFingerprint.SetCurrentActivityResolver(() => CrossCurrentActivity.Current.Activity);
            CardsViewRenderer.Preserve();
            LoadApplication(new SchoolOrganizer.App());
            if (this.Intent != null)
                OnNewIntent(this.Intent);

        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            WidgetPendingAction pendingAction = null;
            switch (intent?.Action)
            {
                case TimeLineWidget.ITEM_CLICK:
                    long ticks = intent.GetLongExtra(nameof(ClassSquare.Begin), 0);
                    string group = intent.GetStringExtra(nameof(ClassSquare.Subject.Group));
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