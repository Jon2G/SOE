using System;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms.Internals;

[assembly: UsesPermission(Name = Manifest.Permission.ReceiveBootCompleted)]
namespace SOE.Droid.Notifications
{
    [BroadcastReceiver(Exported = true, Enabled = true)]
    [IntentFilter(new[]
    {
        Intent.ActionBootCompleted, Intent.ActionLockedBootCompleted, Intent.ActionReboot,
        "android.intent.action.QUICKBOOT_POWERON","com.htc.intent.action.QUICKBOOT_POWERON"
    })]
    [Preserve]
    public class AutoStart : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (context != null)
            {
                try
                {
                    Intent service = new Intent(context, typeof(NotificationService));
                    Context appContext = NotificationHelper.GetContext(context) ?? context;
                    appContext.StartService(intent);
                    IBinder binder = PeekService(appContext, service);
                    appContext.BindService(service, new ServiceConnection(appContext), Bind.AutoCreate);
                }
                catch (Exception ex)
                {
                    NotificationChannel chanel = NotificationChannel.GetNotificationChannel(context, NotificationChannel.ClassChannelId);
                    chanel?.Notify("Exception", ex?.Message??"No message");
                    Crashes.GenerateTestCrash(); 
                }

            }
            else
            {
                NotificationChannel chanel = NotificationChannel.GetNotificationChannel(context, NotificationChannel.ClassChannelId);
                chanel?.Notify("AutoStart.OnReceive", "context is null");
            }

        }
    }
}