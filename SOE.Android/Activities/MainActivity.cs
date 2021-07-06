using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Webkit;
using Kit.Droid;
using Kit.Droid.Services;
using PanCardView.Droid;
using Plugin.CurrentActivity;
using Plugin.Fingerprint;
using Plugin.Media;
using SOE.API;
using SOE.Droid.Notifications;
using SOE.Interfaces;
using SOE.Models.Scheduler;
using SOE.Models.TaskFirst;
using SOE.Services.ActionResponse;
using SOE.Views.Pages;
using SOE.Widgets;
using Xamarin.Forms;

[assembly: Dependency(typeof(MainActivity))]
namespace SOE.Droid.Activities
{
    [Activity(Label = "SOE", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = false, Exported = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    [IntentFilter(
       actions: new[] { Intent.ActionView },
       Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
       DataPathPattern = ".*", DataPathPrefix = ".*",
       DataHost = APIService.NonHttpsUrl, DataSchemes = new[] { "http", "https" })]
    public class MainActivity : Kit.Droid.Services.MainActivity
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
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            await CrossMedia.Current.Initialize();//
            base.OnCreate(savedInstanceState);
            Kit.Droid.Tools.Init(this, savedInstanceState);
            CrossFingerprint.SetCurrentActivityResolver(() => CrossCurrentActivity.Current.Activity);
            CardsViewRenderer.Preserve();
            LoadApplication(new SOE.App());
            if (this.Intent != null)
                OnNewIntent(this.Intent);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            PendingAction pendingAction = null;
            switch (intent?.Action)
            {
                case Intent.ActionView:
                    string link = intent.DataString;
                    if (!string.IsNullOrEmpty(link))
                    {
                        pendingAction = new UrlAction(link);
                    }
                    break;
                case ToDosWidget.ITEM_CLICK:
                    if (Guid.TryParse(intent.GetStringExtra(nameof(ToDo.Guid)), out Guid TodoId))
                    {
                        pendingAction = new TodoWidgetAction(TodoId);
                    }
                    break;
                case TimeLineWidget.ITEM_CLICK:
                    long ticks = intent.GetLongExtra(nameof(ClassSquare.Begin), 0);
                    string group = intent.GetStringExtra(nameof(ClassSquare.Subject.Group));
                    DayOfWeek dayOfWeek = (DayOfWeek)intent.GetIntExtra(nameof(ClassSquare.Day), 1);
                    pendingAction = new TimeLineWidgetSubjectAction(new DateTime(ticks), group, dayOfWeek);
                    break;
                case TimeLineWidget.DAY_CLICK:
                    DayOfWeek dayclicked = (DayOfWeek)intent.GetIntExtra(nameof(ClassSquare.Day), 1);
                    pendingAction = new TimeLineWidgetDayAction(dayclicked);
                    break;
            }
            if (pendingAction != null)
                MasterPage.ResponseTo(pendingAction);
        }
    }
}