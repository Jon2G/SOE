using System;

namespace SOE.Services.ActionResponse
{
    public class TimeLineWidgetDayAction : PendingAction
    {
        public readonly DayOfWeek Day;
        public TimeLineWidgetDayAction(DayOfWeek Day)
        {
            this.Day = Day;
        }
    }
}
