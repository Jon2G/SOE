using SOE.Models.TodoModels;
using SOE.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;

namespace SOE.Notifications.Alarms
{
    public abstract class TodoAlarm : Alarm
    {
        public override string Name => "Notificaciones dias antes de la entrega de tus actividades.";
        public override string Description => "Te notificaremos y recordaremos sobre tus actividades programadas";
        protected uint GetProgrammedId(ToDo todo)
        {
            if (todo.Index <= 0)
            {
                todo.SetNextIndex();
            }
            return Convert.ToUInt32($"{LocalNotification.ClassTimeCode}{todo.Subject.Id}{todo.Index}");
        }
        public override void ScheduleAlll()
        {
            Channel?.Register();
            List<ToDo> todos = ToDosWidget.GetTasks();
            if (todos is null)
            {
                return;
            }
            foreach (ToDo todo in todos.Where(x => ToDo.DaysLeft(x) > 0))
            {
                DateTime date = todo.Date.Add(todo.Time);
                TinyIoC.TinyIoCContainer.Current.Resolve<LocalNotification>()
                    .Set(todo.Title,
                        $"{todo.Subject.Name} - {todo.Subject.Group}\n{todo.Description}",
                        GetProgrammedId(todo), Xamarin.Forms.Color.FromHex(todo.Subject.Color), date.AddDays(-1), this.Channel,"ToDo")
                    .Schedule();
            }
            this.SetMidnightService();
        }

        public abstract void ReSheduleTask(ToDo todo);
    }
}
