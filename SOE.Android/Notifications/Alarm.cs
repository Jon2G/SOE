using Acr.UserDialogs.Builders;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Kit.Droid;
using Microsoft.AppCenter.Crashes;
using SOE.Notifications;
using System;
using Xamarin.Forms.Internals;
using Exception = Java.Lang.Exception;

[assembly: UsesPermission(Name = Manifest.Permission.WakeLock)]
namespace SOE.Droid.Notifications
{
    [BroadcastReceiver(Exported = true, Enabled = true)]
    [IntentFilter(new[] { LocalNotification.START_ALARM })]
    [Preserve]
    public class Alarm : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (context is null)
            {
                return;
            }
            if (intent?.HasExtra(nameof(Notification)) ?? false)
            {
                Notification notification = Notification.FromExtras(intent.Extras, context);
                if (notification.IsOnTime)
                    notification.Notify();
                intent.Extras.Clear();
            }
            try
            {
                if (!context.IsServiceRunning(typeof(NotificationService)))
                {
                    Intent service = new Intent(context, typeof(NotificationService));
                    Context appContext = NotificationHelper.GetContext(context) ?? context;
                    appContext.StartService(intent);
                    IBinder binder = PeekService(appContext, service);
                    appContext.BindService(service, new ServiceConnection(appContext), Bind.AutoCreate);
                }

            }
            catch (Exception ex)
            {
                Crashes.GenerateTestCrash();
                NotificationChannel chanel = NotificationChannel.GetNotificationChannel(context, NotificationChannel.ClassChannelId);
                chanel?.Notify("OnReceive", $"{intent?.Action}");
            }
        }


        internal static void ProgramFor(Bundle extras, DateTime date, Context context, uint requestId)
        {
            long trigger_milis = date.ToUniversalTime().ToUnixTimestamp();
            Intent i = new Intent(context, typeof(Alarm));
            i.PutExtras(extras);
            PendingIntent pi = PendingIntent.GetBroadcast(context, (int)requestId, i, PendingIntentFlags.Immutable);
            AlarmManager am = (AlarmManager)context.GetSystemService(Context.AlarmService);
            am.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, trigger_milis, pi);
        }
        internal static void ProgramFor(Notification notification, DateTime date, Context context, uint requestId, IChannel channel)
        {
            ProgramFor(notification.ToExtras(channel), date, context, requestId);
        }

        public void Cancel(Context context)
        {
            Intent intent = new Intent(context, typeof(Alarm));
            PendingIntent sender = PendingIntent.GetBroadcast(context, 0, intent, 0);
            AlarmManager alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);
            alarmManager.Cancel(sender);
        }
    }
}