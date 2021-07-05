using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Kit.Droid;
using Plugin.CurrentActivity;
using SOE.Droid.Notifications;
using SOE.Interfaces;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(SOE.Droid.Notifications.StartNotificationsService))]
namespace SOE.Droid.Notifications
{
    public class StartNotificationsService : IStartNotificationsService
    {
        void IStartNotificationsService.StartNotificationsService()
        {
            var activity = CrossCurrentActivity.Current.AppContext;
            if (!activity.IsServiceRunning(typeof(NotificationService)))
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.Q)
                {
                    activity.StartService(new Intent(activity, typeof(NotificationService)));

                }
                else
                {
                    activity.StartForegroundService(new Intent(activity, typeof(NotificationService)));
                    activity.StartService(new Intent(activity, typeof(NotificationService)));
                }
            }

        }
    }
}