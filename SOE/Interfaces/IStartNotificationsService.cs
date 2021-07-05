using System;
using System.Collections.Generic;
using System.Text;
using SOE.Models.TaskFirst;

namespace SOE.Interfaces
{
    public interface IStartNotificationsService
    {
        void StartNotificationsService();
        void ReSheduleTask(ToDo toDo);
    }
}
