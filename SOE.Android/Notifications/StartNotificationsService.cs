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
using SOE.Droid.Notifications.Alarms;
using SOE.Interfaces;
using Xamarin.Forms;
using SOE.Models.TodoModels;

[assembly: Xamarin.Forms.Dependency(typeof(SOE.Droid.Notifications.StartNotificationsService))]
namespace SOE.Droid.Notifications
{
    [Xamarin.Forms.Internals.Preserve]
    public class StartNotificationsService : IStartNotificationsService
    {
        public void ReSheduleTask(ToDo toDo)
        {
            ToDoAlarm alarm = new ToDoAlarm();
            alarm.ReSheduleTask(toDo);
        }

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