using System;
using System.Net;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Extensions;
using Android.OS;
using Android.Webkit;
using Firebase.DynamicLinks;
using Kit.Droid;
using Kit.Droid.Services;
using PanCardView.Droid;
using Plugin.CurrentActivity;
using Plugin.Fingerprint;
using Plugin.Media;
using SOE.API;
using SOE.Droid.FireBase;
using SOE.Droid.Notifications;
using SOE.Interfaces;
using SOE.Models.Scheduler;
using SOE.Models.TaskFirst;
using SOE.Services.ActionResponse;
using SOE.Views.Pages;
using SOE.Widgets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Xamarin.Forms;

[assembly: Dependency(typeof(MainActivity))]
namespace SOE.Droid.Activities
{
    [Activity(Label = "SOE", Icon = "@mipmap/icon", Theme = "@style/MainTheme",
        MainLauncher = false, Exported = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    [IntentFilter(
       actions: new[] { Intent.ActionView },
       Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
       DataPathPattern = ".*", DataPathPrefix = ".*",
       DataHost = APIService.NonProdUrl, DataSchemes = new[] { "http", "https" })]

    [IntentFilter(
        actions: new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataPathPattern = ".*", DataPathPrefix = ".*",
        DataHost = SOE.FireBase.Firebase.DynamicLinkHost, DataSchemes = new[] { "http", "https" })]

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
            ServicePointManager.ServerCertificateValidationCallback += ServerCertificateValidationCallback;
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

        private bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
        {
            return true;
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            FirebaseDynamicLinks.Instance.GetDynamicLink(intent)
                 .AddOnSuccessListener(this, new OnSuccessListener())
                 .AddOnFailureListener(this, new OnFailureListener());

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
                    int subjectId = intent.GetIntExtra(nameof(ClassSquare.Subject.Id), 0);
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
    }
}