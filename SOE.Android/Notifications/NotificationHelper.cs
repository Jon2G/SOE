using Android.Content;
using Android.OS;
using Kit.Droid.Services;
using Plugin.CurrentActivity;
using SOE.Notifications;
using System;
using Xamarin.Forms.Internals;

namespace SOE.Droid.Notifications
{
    [Preserve]
    public static class NotificationHelper
    {
        public static NotificationChannel Native(this IChannel channel)
        {
            Context context = GetContext();
            return NotificationChannel.GetNotificationChannel(context, channel.ChannelId);
        }
        public static Context GetContext()
        {
            return MainActivity.GetAppContext() ?? CrossCurrentActivity.Current.AppContext;
        }
        public static NotificationChannel GetChannel(this SOE.Notifications.Alarms.Alarm alarm)
        {
            Context context = GetContext();
            NotificationChannel chanel = new NotificationChannel(context, NotificationChannel.ClassChannelId
                , alarm.Name,
                alarm.Description);
            if ((Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.O) && !chanel.HasBeenRegistered())
            {
                chanel.Register(context);
            }

            return chanel;
        }
        public static void MidnightService(this SOE.Notifications.Alarms.Alarm alarm)
        {
            Context context = GetContext();
            Bundle extras = new Bundle(1);
            extras.PutInt(nameof(Notification.MidnightCode), Notification.MidnightCode);
            Alarm.ProgramFor(extras, DateTime.Today.Date.AddDays(1), context, Notification.MidnightCode);
        }
    }
}