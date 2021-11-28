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
            Context context = GetContext(null);
            return NotificationChannel.GetNotificationChannel(context, channel.ChannelId);
        }
        public static Context GetContext(Context context)
        {
            Context appContext=null;
            if (context is not null)
            {
                appContext = context.ApplicationContext;
            }
            if (appContext is null)
            {
                appContext = MainActivity.GetAppContext() ?? CrossCurrentActivity.Current?.AppContext??context;
             }
            return appContext;
        }
        public static NotificationChannel GetChannel(this SOE.Notifications.Alarms.Alarm alarm)
        {
            Context context = GetContext(null);
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
            Context context = GetContext(null);
            Bundle extras = new Bundle(1);
            extras.PutInt(nameof(Notification.MidnightCode), Notification.MidnightCode);
            DateTime midnight = DateTime.Today.Date.AddDays(1).AddMinutes(10);
            Alarm.ProgramFor(extras, midnight, context, Notification.MidnightCode);
        }
    }
}