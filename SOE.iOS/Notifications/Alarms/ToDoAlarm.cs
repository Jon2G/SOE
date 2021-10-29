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
    public class ToDoAlarm : SOE.Notifications.Alarms.TodoAlarm
    {
        protected override Type NotificationType => typeof(Notification);
        protected override void SetMidnightService()
        {
            throw new NotImplementedException();
        }

    }
}