using Foundation;
using SOE.iOS.Notifications.Alarms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace SOE.iOS.Notifications
{
    [Preserve]
    public class NotificationService
    {
        public NotificationService()
        {

        }
        private void ScheduleAll()
        {
            ClassAlarm alarm = new ClassAlarm();
            alarm.ScheduleAlll();

            ToDoAlarm todo = new ToDoAlarm();
            alarm.ScheduleAlll();
        }


    }
}