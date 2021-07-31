using System;

namespace SOE.Services.ActionResponse
{
    public class TimeLineWidgetSubjectAction : PendingAction
    {
        public readonly DateTime Date;
        public readonly string Group;
        public readonly DayOfWeek Day;

        public TimeLineWidgetSubjectAction(DateTime Date, string Group, DayOfWeek Day)
        {
            this.Date = Date;
            this.Group = Group;
            this.Day = Day;
        }
    }
}
