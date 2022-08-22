using System;
using System.Threading.Tasks;

namespace SOE.Notifications.Alarms
{
    public abstract class Alarm
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract Task ScheduleAll();
        protected virtual void SetMidnightService() { }
        protected virtual IChannel? Channel { get; }
        protected abstract Type NotificationType { get; }

        public Alarm()
        {
            this.TinyRegister();
        }
        private void TinyRegister()
        {
            Kit.Tools.Container.Register(typeof(LocalNotification), this.NotificationType);
        }
    }
}
