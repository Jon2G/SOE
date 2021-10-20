using SOE.Models.TodoModels;

namespace SOE.Interfaces
{
    public interface IStartNotificationsService
    {
        void StartNotificationsService();
        void ReSheduleTask(ToDo toDo);
    }
}
