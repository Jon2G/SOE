using System;

namespace SOE.Notifications
{
    public abstract class LocalNotification
    {
        public const string START_ALARM = "LocalNotifications.START_ALARM";
        public const string ClassTimeCode = "80";
        public uint Index { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Xamarin.Forms.Color Color { get; set; }
        public DateTime Date { get; set; }
        public IChannel Channel { get;  set; }
        public LocalNotification()
        {

        }

        public abstract void Schedule();
        public abstract void Notify();

        public LocalNotification Set(string title, string body, uint index, Xamarin.Forms.Color color, DateTime notifyTime,IChannel channel)
        {
            this.Title = title;
            this.Content = body;
            this.Index = index;
            this.Color = color;
            this.Date = notifyTime;
            this.Channel = channel;
            return this;
        }

    
    }
}
