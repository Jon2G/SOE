using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Extensions;
using Android.OS;
using Firebase.DynamicLinks;

namespace SOE.Droid.Activities
{
    [Activity(Label = "SOE", Theme = "@style/SplashTheme",
        MainLauncher = true, NoHistory = true, ConfigurationChanges = ConfigChanges.ScreenSize)]
    public class SplashActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }

        protected override async void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            var dlink = await FirebaseDynamicLinks.Instance.GetDynamicLink(intent);
        }
    }
}