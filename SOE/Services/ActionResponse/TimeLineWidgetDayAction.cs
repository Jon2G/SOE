using Kit;
using SOE.FireBase;
using SOE.Models.Scheduler;
using SOE.Views.Pages;
using SOE.Views.ViewItems.ScheduleView;
using System;
using System.Threading.Tasks;

namespace SOE.Services.ActionResponse
{
    public class TimeLineWidgetDayAction : IActionResponse
    {
        public readonly DayOfWeek Day;
        public TimeLineWidgetDayAction(DayOfWeek Day)
        {
            this.Day = Day;
        }

        public async Task Execute()
        {
            await Task.Run(() => { while (ScheduleViewMain.Instance is null) { } });
            MasterPage.Instance.Model.SelectedIndex = 2;
            SheduleDay sheduleDay = new SheduleDay(new Day(Day.GetNearest()));
            ScheduleViewMain.Instance.OnDayTappedCommand.Execute(sheduleDay);
        }
    }
}
