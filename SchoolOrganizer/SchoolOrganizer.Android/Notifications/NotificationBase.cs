using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AndroidX.Core.App;
using SchoolOrganizer.Droid.Activities;

namespace SchoolOrganizer.Droid.Notifications
{
    public abstract class NotificationBase
    {
        protected readonly Context Context;

        protected NotificationBase(Context Context)
        {
            this.Context = Context;
        }

        /* when your phone is locked screen wakeup method*/
        protected void WakeUpScreen()
        {
            PowerManager pm = (PowerManager) this.Context.GetSystemService(Context.PowerService);
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

        protected void Notify(int notificationIndex, NotificationCompat.Builder builder, NotificationChannel chanel)
        {
            chanel.RegisterNotificationChannel(Context);
            WakeUpScreen();
            addNotification(notificationIndex, builder, chanel);
        }

        /*Add notification method use for add icon and title*/
        private void addNotification(int notificationIndex, NotificationCompat.Builder builder,
            NotificationChannel chanel)
        {
            builder.SetAutoCancel(true)
                .SetOngoing(true)
                .SetChannelId(chanel.ChannelId)
                .SetPriority(NotificationCompat.PriorityHigh)
                .SetVisibility(NotificationCompat.VisibilityPublic);

            Notification notification = new Notification();

            // Cancel the notification after its selected
            notification.Flags |= NotificationFlags.AutoCancel;

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
            chanel.NotificationManager.Notify(notificationIndex, builder.Build());

        }
    }
}