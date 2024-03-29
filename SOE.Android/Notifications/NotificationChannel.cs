﻿using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using AndroidX.Core.App;
using SOE.Droid.Notifications.Alarms;
using SOE.Notifications;
using Xamarin.Forms.Internals;

namespace SOE.Droid.Notifications
{
    [Preserve]
    public class NotificationChannel : IChannel
    {
        public const string ToDoChannelId = "todo_channel_id_school_organizer";
        public const string ClassChannelId = "class_channel_id_school_organizer";
        public const string BackgroundServiceId = "class_channel_id_soe_background_service";
        public string ChannelId { get; }
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

        public void RegisterSilently()
        {
            // Create the NotificationChannel, but only on API 26+ because
            // the NotificationChannel class is new and not in the support library
            //if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            //{
            Android.App.NotificationChannel channel =
                new Android.App.NotificationChannel(this.ChannelId, this.Name, NotificationImportance.None)
                {
                    Description = Description,
                    LockscreenVisibility = NotificationVisibility.Secret
                };
            // Register the channel with the system; you can't change the importance
            // or other notification behaviors after this
            this.NotificationManager.CreateNotificationChannel(channel);
            this.IsRegistered = true;
            //}
        }
        public void Register()
        {
            this.Register(this.Context);
        }
        public void Register(Context context, NotificationImportance Importance = NotificationImportance.High)
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
            channel.SetVibrationPattern(new long[] { 200, 200 });
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
            if (context is null)
            {
                return null;
            }
            if (Android.OS.Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                //no existe antes de OREO
                return null;
            }
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
            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                //no existe antes de OREO
                this.Register(this.Context);
            }

            NotificationCompat.Builder builder;
            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                builder = new NotificationCompat.Builder(this.Context, this.ChannelId);
                builder.SetChannelId(this.ChannelId);
            }
            else
            {
#pragma warning disable CS0618 // Type or member is obsolete
                builder = new NotificationCompat.Builder(this.Context);
#pragma warning restore CS0618 // Type or member is obsolete
            }
            builder.SetSmallIcon(Android.Resource.Id.Icon)
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
            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                this.NotificationManager.Notify(Index++, builder.Build());
            }
            else
            {
                builder.Build().Notify();
            }
        }
    }
}
