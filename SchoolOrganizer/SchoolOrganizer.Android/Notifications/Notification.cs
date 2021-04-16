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
using AndroidX.Core.App;
using SchoolOrganizer.Notifications;
using Xamarin.Forms.Platform.Android;
using Android.Util;
using SchoolOrganizer.Droid.Activities;

namespace SchoolOrganizer.Droid.Notifications
{
    public class Notification
    {
        private readonly string Title;
        private readonly string Content;
        private readonly int Index;
        private readonly Xamarin.Forms.Color Color;
        private readonly Context Context;
        private readonly NotificationChannel NotificationChannel;
        public const string TitleKey = nameof(Title);
        public const string ContentKey = nameof(Content);
        public const string IndexKey = nameof(Index);
        public const string ColorKey = nameof(Color);
        public const string NotificationChanelIdKey = nameof(Notifications.NotificationChannel.ChannelId);
        public Notification(string Title, string Content, int Index, string Color, Context Context,
            NotificationChannel NotificationChannel)
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
            this.Context = Context;
            this.NotificationChannel = NotificationChannel;
        }
        public Android.App.Notification Build()
        {
            var color = this.Color.ToAndroid();
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this.Context, this.NotificationChannel.ChannelId)
                .SetSmallIcon(Resource.Drawable.xamagonblue)
                .SetContentTitle(this.Title)
                .SetContentText(this.Content)
                .SetStyle(new NotificationCompat.BigTextStyle()
                    .BigText(this.Content)
                    .SetBigContentTitle(this.Title))
                .SetColorized(true)
                .SetColor(color)
                .SetAutoCancel(true)
                .SetOngoing(true)
                .SetChannelId(this.NotificationChannel.ChannelId)
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
            PendingIntent contentIntent = PendingIntent.GetActivity(this.Context, 0, notificationIntent,
                PendingIntentFlags.UpdateCurrent);
            builder.SetContentIntent(contentIntent);
            // Add as notification
            return BuildedNotification;
        }

        public Bundle ToExtras()
        {
            Bundle bundle = new Bundle();
            bundle.PutString(nameof(Notification), nameof(Notification));
            bundle.PutString(TitleKey, this.Title);
            bundle.PutString(ContentKey, this.Content);
            bundle.PutInt(IndexKey, this.Index);
            bundle.PutString(ColorKey, this.Color.ToHex());
            bundle.PutString(NotificationChanelIdKey, this.NotificationChannel.ChannelId);
            return bundle;
        }
        internal static Notification FromExtras(Bundle extras, Context context)
        {
            return new Notification(
                extras.GetString(TitleKey),
                extras.GetString(ContentKey),
                extras.GetInt(IndexKey),
                extras.GetString(ColorKey),
                context,
                NotificationChannel.GetNotificationChannel(context, extras.GetString(NotificationChanelIdKey)));
        }
        /* when your phone is locked screen wakeup method*/
        protected void WakeUpScreen()
        {
            PowerManager pm = (PowerManager)this.Context.GetSystemService(Context.PowerService);
            bool isScreenOn = pm.IsScreenOn;

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

        public void Notify()
        {
            if (!this.NotificationChannel.IsRegistered)
            {
                this.NotificationChannel.RegisterNotificationChannel(Context);
            }
            WakeUpScreen();
            NotificationChannel.NotificationManager.Notify(this.Index, this.Build());
        }

    }
}