using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Firebase.DynamicLinks;
using PanCardView.Droid;
using Plugin.CurrentActivity;
using Plugin.Fingerprint;
using Plugin.Media;
using SOE.Droid.FireBase;
using SOE.FireBase;
using SOE.Models.Scheduler;
using SOE.Models.TodoModels;
using SOE.Secrets;
using SOE.Services.ActionResponse;
using SOE.Views.Pages;
using SOE.Widgets;
using System;
using Xamarin.Forms;
using Uri = Android.Net.Uri;

namespace SOE.Droid.Activities
{
    [Android.Runtime.Preserve]
    [Activity(Label = "SOE", Icon = "@mipmap/icon", Theme = "@style/MainTheme",
        MainLauncher = false, Exported = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    [IntentFilter(
       actions: new[] { Intent.ActionView },
       Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
       DataPathPattern = ".*", DataPathPrefix = ".*",
       DataHost = DotNetEnviroment.NonHttpsUrl, DataSchemes = new[] { "http", "https" })]

    [IntentFilter(
        actions: new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataPathPattern = ".*", DataPathPrefix = ".*",
        DataHost = SOE.FireBase.Firebase.DynamicLinkHost, DataSchemes = new[] { "http", "https" })]
    [MetaData(name: "com.google.android.actions", Resource = "@xml/actions")]

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

        public override bool OnKeyUp([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if (e.Flags.HasFlag(Android.Views.KeyEventFlags.SoftKeyboard) || keyCode == Keycode.Back)
                return base.OnKeyUp(keyCode, e);
            return false;
        }
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            await CrossMedia.Current.Initialize();//
            base.OnCreate(savedInstanceState);
            CrossFingerprint.SetCurrentActivityResolver(() => CrossCurrentActivity.Current.Activity);
            CardsViewRenderer.Preserve();
            LoadApplication(new App());
            Initialize(savedInstanceState, typeof(App));
            if (this.Intent != null)
                OnNewIntent(this.Intent);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            OnSuccessListener successListener = new OnSuccessListener(new Command<IActionResponse>(MasterPage.ResponseTo));
            FirebaseDynamicLinks.Instance.GetDynamicLink(intent)
                .AddOnSuccessListener(this, successListener);
            string link = null;
            IActionResponse pendingAction = null;
            switch (intent?.Action)
            {
                case "actions.intent.CREATE_THING":
                    link = intent.DataString;
                    pendingAction = OnCreateThing(link);
                    break;
                case Intent.ActionView:
                    link = intent.DataString;
                    if (!string.IsNullOrEmpty(link))
                    {
                        Uri uri = Uri.Parse(link);
                        successListener.FromDeepLink(uri);
                    }
                    break;
                case ToDosWidget.ITEM_CLICK:
                    pendingAction = new TodoWidgetAction(intent.GetStringExtra(nameof(ToDo.DocumentId)));
                    break;
                case TimeLineWidget.ITEM_CLICK:
                    long ticks = intent.GetLongExtra(nameof(ClassSquare.Begin), 0);
                    string subjectId = intent.GetStringExtra(nameof(ClassSquare.Subject.DocumentId));
                    DayOfWeek dayOfWeek = (DayOfWeek)intent.GetIntExtra(nameof(ClassSquare.Day), 1);
                    pendingAction = new TimeLineWidgetSubjectAction(new DateTime(ticks), subjectId, dayOfWeek);
                    break;
                case TimeLineWidget.DAY_CLICK:
                    DayOfWeek dayclicked = (DayOfWeek)intent.GetIntExtra(nameof(ClassSquare.Day), 1);
                    pendingAction = new TimeLineWidgetDayAction(dayclicked);
                    break;
            }

            if (pendingAction != null)
            {
                MasterPage.ResponseTo(pendingAction);
            }

        }

        private IActionResponse OnCreateThing(string link)
        {
            Uri uri = Uri.Parse(link);
            string name = uri.GetQueryParameter("name");
            string description = uri.GetQueryParameter("description");
            IActionResponse action = null;
            switch (name?.ToLower()?.Trim())
            {
                case "tarea":
                    action = new CreateToDoAction(description);
                    break;
                case "recordatorio":
                    action = new CreateReminderAction(description);
                    break;

            }
            return action;
        }


    }
}