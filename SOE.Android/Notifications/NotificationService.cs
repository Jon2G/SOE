using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Content;
using AndroidX.Core.App;
using SOE.Droid.Notifications.Alarms;
using Xamarin.Forms.Internals;

namespace SOE.Droid.Notifications
{
    [Service(Enabled = true, Exported = true)]
    [IntentFilter(new[] { ".NotificationService" })]
    [Preserve]
    public class NotificationService : Service
    {
        private static AlarmBinder alarmBinder = new();

        public override ComponentName? StartForegroundService(Intent? service)
        {
            return base.StartForegroundService(service);
        }

        public override IBinder OnBind(Intent intent)
        {
            alarmBinder.OnBind(this);
            return alarmBinder;
        }

        public void StartCommand()
        {
            ForceForeground();
            ScheduleAll();
            StopSelf();
        }

        private void ForceForeground()
        {
            Android.App.Notification notification;
            // API lower than 26 do not need this work around.
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                Intent intent = new Intent(this, typeof(NotificationService));
                // service has already been initialized.
                // startForeground method should be called within 5 seconds.
                ContextCompat.StartForegroundService(this, intent);
                NotificationChannel channel = new(this.ApplicationContext, NotificationChannel.BackgroundServiceId,
                    "Servicio de notificaciones", "Servicio para la programaición de notificaciones recurrentes");
                if (!channel.HasBeenRegistered())
                {
                    channel.RegisterSilently();
                }
                Android.App.Notification.Builder builder = new Android.App.Notification.Builder(this, channel.ChannelId)
                        .SetContentTitle("SOE")
                        .SetContentText("SOE Running")
                        .SetAutoCancel(true);
                notification = builder.Build();

            }
            else
            {
#pragma warning disable CS0618 // Type or member is obsolete
                NotificationCompat.Builder builder = new NotificationCompat.Builder(this)
#pragma warning restore CS0618 // Type or member is obsolete
                        .SetContentTitle("SOE")
                        .SetContentText("SOE is Running...")
                        .SetPriority(NotificationCompat.PriorityDefault)
                        .SetAutoCancel(true);
                notification = builder.Build();
            }
            // call startForeground just after startForegroundService.
            StartForeground(Notification.ServiceNotificationId, notification);
        }




        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            base.OnStartCommand(intent, flags, startId);
            return StartCommandResult.NotSticky;
        }
        public override void OnCreate()
        {
            base.OnCreate();
            this.StartCommand();
        }

        public void ScheduleAll()
        {
            try
            {
                ClassAlarm alarm = new();
                alarm.ScheduleAll();
                ToDoAlarm todo = new();
                todo.ScheduleAll();
            }
            catch (System.Exception ex)
            {
                Kit.Log.Logger?.Error(ex, "ScheduleAll");
            }

        }
    }
}