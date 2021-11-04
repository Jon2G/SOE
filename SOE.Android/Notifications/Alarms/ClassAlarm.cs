using SOE.Notifications;
using System;
using Xamarin.Forms.Internals;

namespace SOE.Droid.Notifications.Alarms
{
    [Preserve]
    public class ClassAlarm : SOE.Notifications.Alarms.ClassAlarm
    {
        protected override Type NotificationType => typeof(Notification);
        protected override void SetMidnightService() => this.MidnightService();
        protected override IChannel Channel => this.GetChannel();

        public ClassAlarm()
        {
            
        }
    }
}