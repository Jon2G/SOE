using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolOrganizer.Notifications
{
    public interface ILocalNotificationService
    {
        void Start();
        //void LocalNotification(string title, string body, int id, DateTime notifyTime);
        void Cancel();
    }
}
