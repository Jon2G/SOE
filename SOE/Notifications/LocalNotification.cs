using System;

namespace SOE.Notifications
{
    public class LocalNotification
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime NotifyTime { get; set; }
    }
}
