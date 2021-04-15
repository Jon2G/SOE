using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.Telephony.Gsm;
using Java.Security;
using Plugin.CurrentActivity;
using SchoolOrganizer.Droid.Notifications;
using SchoolOrganizer.Notifications;
[assembly: Xamarin.Forms.Dependency(typeof(NotificationService))]
namespace SchoolOrganizer.Droid.Notifications
{
    [Service(Enabled = true, Exported = true)]
    [IntentFilter(new[] { ".NotificationService" })]
    public class NotificationService : Service
    {
        private Context _Context;
        private Context Context
        {
            get => _Context ??= GetAppContext() ?? this.ApplicationContext;
            set => _Context = value;
        }
        public static Alarm Alarm { get; private set; }
        public override IBinder? OnBind(Intent? intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            NotificationChannel chanel = NotificationChannel.GetNotificationChannel(this.Context, NotificationChannel.ClassChannelId);
            chanel?.Notify("NotificationService.OnStartCommand", intent?.Action ?? "No action", 6);
            if (Alarm is null)
            {
                OnCreate();
            }
            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void OnTaskRemoved(Intent? rootIntent)
        {
            NotificationChannel chanel = NotificationChannel.GetNotificationChannel(this.Context, NotificationChannel.ClassChannelId);
            chanel?.Notify("NotificationService.OnTaskRemoved", rootIntent?.Action ?? "No action", 8);

            // TODO Auto-generated method stub
            Intent restartService = new Intent(this.Context,
                typeof(NotificationService));
            restartService.SetPackage(PackageName);
            PendingIntent restartServicePI =
                PendingIntent.GetService(this.Context, 1,
                    restartService, PendingIntentFlags.OneShot);

            //Restart the service once it has been killed android
            AlarmManager alarmService = (AlarmManager)this.Context.GetSystemService(Context.AlarmService);
            alarmService.Set(AlarmType.ElapsedRealtime, SystemClock.ElapsedRealtime() + 100, restartServicePI);

        }

        public override void OnCreate()
        {
            this.Context = GetAppContext();
            base.OnCreate();
            NotificationChannel chanel = NotificationChannel.GetNotificationChannel(this.Context, NotificationChannel.ClassChannelId);
            chanel?.Notify("NotificationService.OnCreate", "No action", 7);
            //start a separate thread and start listening to your network object
            Alarm = new Alarm();
            Alarm.Start(this.Context);

        }
        public static Context GetAppContext()
        {
            return Android.App.Application.Context;
        }
    }
}