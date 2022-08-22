using Kit;
using SOE.Models.TodoModels;
using SOE.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOE.Notifications.Alarms
{
    public abstract class TodoAlarm : Alarm
    {
        public override string Name => "Notificaciones dias antes de la entrega de tus actividades.";
        public override string Description => "Te notificaremos y recordaremos sobre tus actividades programadas";
        protected uint GetProgrammedId(ToDo todo)
        {
            ProgrammedNotifications? programmed =
                ProgrammedNotifications.Get(Enums.NotificationType.TODO, todo.DocumentId);
            if (programmed is null)
            {
                programmed = ProgrammedNotifications.Save(Enums.NotificationType.TODO, todo.DocumentId);
            }
            return Convert.ToUInt32($"{LocalNotification.ClassTimeCode}{programmed.Id}");
        }
        public override async Task ScheduleAll()
        {
            await Task.Yield();
            Channel?.Register();
            List<ToDo> todos = await ToDosWidget.GetTasks();
            if (todos is null)
            {
                return;
            }
            foreach (ToDo todo in todos.Where(x => ToDo.DaysLeft(x) > 0))
            {
                if (todo is null)
                {
                    continue;
                }
                await todo.GetSubject();
                if (todo.Subject is null)
                {
                    continue;
                }
                await todo.Subject.GetGroup();
                if (todo.Subject.Group is null)
                {
                    continue;
                }
                IChannel? channel = this.Channel;
                if (channel is null) { continue; }
                DateTime date = todo.Date.Add(todo.Time);
                Kit.Tools.Container.Get<LocalNotification>()?
                    .Set(todo.Title,
                        $"{todo.Subject.Name} - {todo.Subject.Group.Name}\n{todo.Description}",
                        GetProgrammedId(todo), Xamarin.Forms.Color.FromHex(todo.Subject.Color), date.AddDays(-1), channel, "ToDo")?
                    .Schedule();
            }
            this.SetMidnightService();
        }

        public abstract Task ReSheduleTask(ToDo todo);
    }
}
