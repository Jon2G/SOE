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
using Android.Graphics;
using AndroidX.Core.App;
using Kit;

namespace SchoolOrganizer.Droid.Notifications
{
    public class NotificationChannel
    {
        public const string ClassChannelId = "class_channel_id_school_organizer";
        public readonly string ChannelId;
        public readonly string Name;
        public readonly string Description;
        private readonly Context Context;
        public NotificationManager NotificationManager { get; private set; }
        public NotificationChannel(Context Context, string ChannelId, string Name, string Description)
        {
            this.Name = Name;
            this.Description = Description;
            this.Context = Context;
            this.ChannelId = ChannelId;
            this.NotificationManager = GetNotificationManager(this.Context);
        }
        public void RegisterNotificationChannel(Context context, NotificationImportance Importance = NotificationImportance.High)
        {
            // Create the NotificationChannel, but only on API 26+ because
            // the NotificationChannel class is new and not in the support library
            //if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            //{
            Android.App.NotificationChannel channel =
                new Android.App.NotificationChannel(this.ChannelId, Name, Importance)
                {
                    Description = Description,
                    LightColor = (Color.Red),
                    LockscreenVisibility = NotificationVisibility.Public

                };
            channel.EnableVibration(true);
            channel.SetVibrationPattern(new long[] { 1000, 2000 });
            channel.EnableLights(true);
            channel.EnableVibration(true);
            // Register the channel with the system; you can't change the importance
            // or other notification behaviors after this

            this.NotificationManager.CreateNotificationChannel(channel);
            this.IsRegistered = true;
            //}
        }



        private static NotificationManager GetNotificationManager(Context Context)
        {
            return (NotificationManager)Context.GetSystemService(Java.Lang.Class.FromType(typeof(NotificationManager)));
        }

        public static NotificationChannel GetNotificationChannel(Context context, string ChannelId)
        {
            var mgr = GetNotificationManager(context);
            var chanel = mgr?.GetNotificationChannel(ChannelId);

            if (chanel is null)
            {
                return null;
            }

            return new NotificationChannel(context, chanel.Id, chanel.Name, chanel.Description);
        }
        public bool IsRegistered { get; private set; }
        public bool HasBeenRegistered()
        {
            return IsRegistered = this.NotificationManager.GetNotificationChannel(this.ChannelId) != null;
        }

        private static int Index = 10;
        public void Notify(string text, string content)
        {
            this.RegisterNotificationChannel(this.Context);
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this.Context, this.ChannelId)
                .SetSmallIcon(Android.Resource.Id.Icon)
                .SetContentTitle("Notifications Debugger")
                .SetVisibility(NotificationCompat.VisibilityPublic)
                .SetContentText(text)
                .SetStyle(new NotificationCompat.BigTextStyle()
                    .BigText(content)
                    .SetBigContentTitle(text))
                .SetPriority(NotificationCompat.PriorityHigh)
                .SetColorized(true)
                .SetColor(Color.Red)
                .SetOngoing(false)
                .SetLights(Color.Red, 300, 100)
                .SetVibrate(new long[] { 0, 1000, 200, 1000 });
            if (Tools.Debugging)
                this.NotificationManager.Notify(Index++, builder.Build());
        }
    }
}