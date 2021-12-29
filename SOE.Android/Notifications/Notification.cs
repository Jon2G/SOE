using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using AndroidX.Core.App;
using SOE.Data;
using SOE.Droid.Activities;
using SOE.Notifications;
using System;
using System.Globalization;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;
using Environment = System.Environment;
using Log = Android.Util.Log;

[assembly: UsesPermission(Name = Manifest.Permission.UseFullScreenIntent)]
[assembly: UsesPermission(Name = Manifest.Permission.SystemAlertWindow)]
namespace SOE.Droid.Notifications
{
    [Preserve]
    public class Notification : LocalNotification
    {
        public const int ServiceNotificationId = 555;
        public const int MidnightCode = 800;
        private Context Context { get;  set; }
        private const string TitleKey = nameof(Title);
        private const string ContentKey = nameof(Content);
        private const string IndexKey = nameof(Index);
        private const string ColorKey = nameof(Color);
        private const string DateKey = nameof(Date);
        private const string DateFormat = "dd-MM-yy HH:mm";
        private const string NotificationChanelIdKey = nameof(Notifications.NotificationChannel.ChannelId);

        public bool IsOnTime
        {
            get
            {
                var now = DateTime.Now;
                if (this.Date <= now)
                {
                    TimeSpan difference = now - this.Date;
                    if (difference.TotalMinutes < 10d)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public Notification()
        {

        }

        public Notification(string Title, string Content, uint Index, string Color, DateTime Date, Context Context,
            IChannel NotificationChannel)
        : this(Title, Content, Index, Color, Date.ToString(DateFormat, CultureInfo.InvariantCulture), Context, NotificationChannel)
        {

        }
        public Notification(string Title, string Content, uint Index, string Color, string Date, Context Context, IChannel NotificationChannel)
        {
            this.Title = Title;
            this.Content = Content;
            this.Index = Index;
            if (!string.IsNullOrEmpty(Color))
            {
                this.Color = Xamarin.Forms.Color.FromHex(Color);
            }
            else
            {
                this.Color = Xamarin.Forms.Color.MidnightBlue;
            }
            this.Date = DateTime.ParseExact(Date, DateFormat, CultureInfo.InvariantCulture);
            this.Context = Context;
            this.Channel = NotificationChannel;
        }
        public Android.App.Notification Build()
        {
            var color = this.Color.ToAndroid();

            NotificationCompat.Builder builder;
            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                builder = new NotificationCompat.Builder(this.Context, this.Channel.ChannelId);
                builder.SetChannelId(this.Channel.ChannelId);
            }
            else
            {
#pragma warning disable CS0618 // Type or member is obsolete
                builder = new NotificationCompat.Builder(this.Context);
#pragma warning restore CS0618 // Type or member is obsolete
            }
            builder
                .SetSmallIcon(Resource.Drawable.logo_soe_fill)
                .SetContentTitle(this.Title)
                .SetContentText(this.Content)
                .SetStyle(new NotificationCompat.BigTextStyle()
                    .BigText(this.Content)
                    .SetBigContentTitle(this.Title))
                .SetColorized(true)
                .SetColor(color)
                .SetAutoCancel(true)
                .SetOngoing(false)
                .SetPriority(NotificationCompat.PriorityHigh)
                .SetVisibility(NotificationCompat.VisibilityPublic);
            var BuildedNotification = builder.Build();
            // Cancel the notification after its selected
            BuildedNotification.Flags |= NotificationFlags.AutoCancel;
            //notification.Defaults |= Notification.DEFAULT_SOUND;
            //notification.Defaults |= Notification.DEFAULT_VIBRATE;
            //builder.setDefaults(notification.defaults);
            /*notification send with type calling */
            Intent notificationIntent = new Intent(this.Context, typeof(SplashActivity));
            //notificationIntent.putExtra("notification_room_id", body);
            //notificationIntent.putExtra("data", dataa);
            //notificationIntent.putExtra("calling", "calling");

            PendingIntent pIntent = PendingIntent.GetActivity(this.Context, 0, new Intent(), PendingIntentFlags.UpdateCurrent);
            PendingIntent contentIntent = PendingIntent.GetActivity(this.Context, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);

            builder.SetFullScreenIntent(pIntent, true);
            builder.SetContentIntent(contentIntent);


            // Add as notification
            return BuildedNotification;
        }

        public Bundle ToExtras(IChannel channel)
        {
            Bundle bundle = new Bundle();
            bundle.PutString(nameof(Notification), nameof(Notification));
            bundle.PutString(TitleKey, this.Title);
            bundle.PutString(ContentKey, this.Content);
            bundle.PutInt(IndexKey, (int)this.Index);
            bundle.PutString(ColorKey, this.Color.ToHex());
            bundle.PutString(DateKey, this.Date.ToString(DateFormat, CultureInfo.InvariantCulture));
            bundle.PutString(NotificationChanelIdKey, channel.ChannelId);
            return bundle;
        }
        public static Notification FromExtras(Bundle extras, Context context)
        {
            return new Notification(
                extras.GetString(TitleKey),
                extras.GetString(ContentKey),
                (uint)extras.GetInt(IndexKey),
                extras.GetString(ColorKey),
                extras.GetString(DateKey, DateTime.MinValue.ToString(DateFormat)),
                context,
                SOE.Droid.Notifications.NotificationChannel.GetNotificationChannel(context,
                    extras.GetString(NotificationChanelIdKey)));
        }

        /* when your phone is locked screen wakeup method*/
        protected void WakeUpScreen()
        {
            PowerManager pm = (PowerManager)this.Context.GetSystemService(Context.PowerService);
            bool isScreenOn = pm.IsInteractive;

            Log.Debug("screen on......", "" + isScreenOn);
            if (!isScreenOn)
            {
                PowerManager.WakeLock wl = pm.NewWakeLock(
                    WakeLockFlags.Full | WakeLockFlags.AcquireCausesWakeup | WakeLockFlags.OnAfterRelease, "MyLock");
                wl.Acquire(10000);
                PowerManager.WakeLock wl_cpu = pm.NewWakeLock(WakeLockFlags.Partial, "MyCpuLock");
                wl_cpu.Acquire(10000);
            }
        }

        public override void Notify()
        {
            if ((Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.O) && !this.Channel.IsRegistered)
            {
                (this.Channel as NotificationChannel)?.Register(this.Context);
            }
            WakeUpScreen();

            if ((Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.O))
            {
                (this.Channel as NotificationChannel)?.NotificationManager.Notify((int)this.Index, this.Build());
            }
            else
            {
                NotificationManager notificationManager = (NotificationManager)this.Context.GetSystemService(
                   Java.Lang.Class.FromType(typeof(Android.App.NotificationManager)));
                notificationManager.Notify((int)this.Index, this.Build());
            }
        }



        public override void Schedule()
        {
            this.Context = NotificationHelper.GetContext(this.Context);
            Alarm.ProgramFor(this, this.Date, this.Context, this.Index, this.Channel);
        }

        public static void Cancel(Context context,int Id)
        {
            NotificationManager notificationManager = (NotificationManager)context.GetSystemService(
                       Java.Lang.Class.FromType(typeof(Android.App.NotificationManager)));
            notificationManager.Cancel(Id);
        }
    }
}