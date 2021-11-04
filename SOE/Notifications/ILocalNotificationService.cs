using SOE.Models.TodoModels;

namespace SOE.Notifications
{
    public interface ILocalNotificationService
    {
        void ScheduleAll();
        void UnScheduleAll();
        void ReSheduleTask(ToDo toDo);
    }
}
