using Forms9Patch.iOS;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using UserNotifications;

namespace SOE.iOS.Notifications
{
    public static class NotificationHelper
    {
        public static void Cancel(this UILocalNotification notification)
        {
            UIApplication.SharedApplication.CancelLocalNotification(notification);
        }

        public static bool GetIndex(this UILocalNotification notification, out uint index)
        {
            index = 0;
            NSObject nsIndex = notification.UserInfo?[nameof(Notification.Index)];
            if (nsIndex is NSNumber nsNumber)
            {
                index = nsNumber.UInt32Value;
                return true;
            }
            return false;
        }
        public static bool MatchIndex(this UILocalNotification notification, uint Index)
        {
            if (notification.GetIndex(out uint nIndex) && nIndex == Index)
            {
                return true;
            }

            return false;
        }
    }
}