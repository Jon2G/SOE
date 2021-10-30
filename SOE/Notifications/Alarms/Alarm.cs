using System;

namespace SOE.Notifications.Alarms
{
    public abstract class Alarm
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract void ScheduleAlll();
        protected virtual void SetMidnightService(){}
        protected virtual IChannel Channel { get; }
        protected abstract Type NotificationType { get; }

        public Alarm()
        {
            this.TinyRegister();
        }
        private void TinyRegister()
        {
            TinyIoC.TinyIoCContainer.Current.Register(typeof(LocalNotification), this.NotificationType);
        }
    }
}
