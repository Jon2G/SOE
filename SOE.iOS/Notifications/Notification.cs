using Foundation;
using SOE.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace SOE.iOS.Notifications
{
    [Preserve]
    public class Notification : LocalNotification
    {
        public UILocalNotification GetNativeNotification()
        {
            // create the notification
            var notification = new UILocalNotification();

            // set the fire date (the date time in which it will fire)
            notification.FireDate = NSDate.FromTimeIntervalSinceNow(60);

            // configure the alert
            notification.AlertAction = "View Alert";
            notification.AlertBody = "Your one minute alert has fired!";

            // modify the badge
            notification.ApplicationIconBadgeNumber = 1;

            // set the sound to be the default sound
            notification.SoundName = UILocalNotification.DefaultSoundName;
            return notification;
        }
        public override void Schedule()
        {
            UILocalNotification notification = this.GetNativeNotification();
            // schedule it

            UIApplication.SharedApplication.ScheduleLocalNotification(notification);
        }

        public override void Notify()
        {
            UILocalNotification notification = this.GetNativeNotification();
            UIApplication.SharedApplication.PresentLocalNotificationNow(notification);
        }
    }
}