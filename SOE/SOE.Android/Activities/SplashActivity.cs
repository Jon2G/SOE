using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace SchoolOrganizer.Droid.Activities
{
    [Activity(Label = "Organizador Escolar", Theme = "@style/SplashTheme",
        MainLauncher = true, NoHistory = true, ConfigurationChanges = ConfigChanges.ScreenSize)]
    public class SplashActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }
    }
}