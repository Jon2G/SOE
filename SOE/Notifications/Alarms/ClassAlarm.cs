using SOE.Models.Scheduler;
using System;
using System.Collections.Generic;

namespace SOE.Notifications.Alarms
{
    public abstract class ClassAlarm : Alarm
    {
        public override string Name => "Notificaciones antes de la hora de tu clase.";
        public override string Description => "Te notificaremos minutos antes de que comience tu clase";

        public ClassAlarm()
        {
            
        }
        public override void ScheduleAlll()
        {
            this.Channel?.Register();
            Day day = Day.Today();
            List<ClassSquare> timeline = day?.GetTimeLine();
            if (timeline is null)
            {
                return;
            }
            DateTime now = DateTime.Now;
            foreach (ClassSquare cl in timeline)
            {
                DateTime begin = day.Date.Add(cl.Begin);
                DateTime end = day.Date.Add(cl.End).AddMinutes(-10);
                //bool InProgress = now <= end && begin <= now;
                bool InProgress = (now.Ticks >= begin.Ticks && now <= end);

                int programmedId = Convert.ToInt32($"{LocalNotification.ClassTimeCode}{cl.Subject.Id}{(int)day.DayOfWeek}");
                DateTime desiredDate = DateTime.MinValue;

                //si ya inicio enviar una notificación ahora!
                if (InProgress)
                {
                    //notification.Index = 0;
                    //notification.Notify();
                }
                else if (begin > now) //si no ha iniciado se debe programar una alerta
                {
                    desiredDate = begin.AddMinutes(-10);
                }
                else if (begin < now)
                {
                    //Program for next week
                    desiredDate = day.Date.AddDays(7).Add(cl.Begin).AddMinutes(-10);
                }

                TinyIoC.TinyIoCContainer.Current.Resolve<LocalNotification>()
                    .Set(cl.Subject.Name,
                    $"{cl.FormattedTime} ,{cl.Subject.Group}\n{(InProgress ? "En curso..." : "Comienza pronto")}",
                    1, Xamarin.Forms.Color.FromHex(cl.Subject.Color), desiredDate, this.Channel)
                    .Schedule();
            }

            this.SetMidnightService();
        }
    }
}
