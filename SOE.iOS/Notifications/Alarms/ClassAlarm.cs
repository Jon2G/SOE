using Foundation;
using SOE.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace SOE.iOS.Notifications.Alarms
{
    [Preserve]
    public class ClassAlarm : SOE.Notifications.Alarms.ClassAlarm
    {
        protected override Type NotificationType => typeof(Notification);

    }
}