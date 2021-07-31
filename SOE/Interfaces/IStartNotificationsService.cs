using SOE.Models.TaskFirst;

namespace SOE.Interfaces
{
    public interface IStartNotificationsService
    {
        void StartNotificationsService();
        void ReSheduleTask(ToDo toDo);
    }
}
