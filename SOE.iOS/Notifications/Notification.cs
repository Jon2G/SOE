using Forms9Patch.iOS;
using Foundation;
using SOE.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace SOE.iOS.Notifications
{
    [Preserve]
    public class Notification : LocalNotification
    {
        public UILocalNotification GetNativeNotification()
        {
            // create the notification
            UILocalNotification notification = new();
            // set the fire date (the date time in which it will fire)

            notification.FireDate = NSDate.FromTimeIntervalSinceNow(60);

            double seconds = (this.Date-DateTime.Now).TotalSeconds;
            notification.FireDate = NSDate.FromTimeIntervalSinceNow(seconds);
            notification.UserInfo = new NSDictionary(new NSString(nameof(Index)), new NSNumber(this.Index));
            // configure the alert
            notification.AlertTitle = this.Title;
            //notification.AlertAction = "View Alert";
            notification.AlertBody = this.Content;

            switch (this.Type)
            {
                case "Class":
                    notification.RepeatInterval = NSCalendarUnit.Week;
                    break;
                case "Inmediate":
                    notification.FireDate = NSDate.FromTimeIntervalSinceNow(60);
                    break;
            }
            // modify the badge
            //notification.ApplicationIconBadgeNumber = 1;


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
            this.Type = "Inmediate";
            UILocalNotification notification = this.GetNativeNotification();
            UIApplication.SharedApplication.PresentLocalNotificationNow(notification);
        }
    }
}