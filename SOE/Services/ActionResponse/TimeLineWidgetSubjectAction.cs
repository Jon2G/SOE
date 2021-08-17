using System;

namespace SOE.Services.ActionResponse
{
    public class TimeLineWidgetSubjectAction : PendingAction
    {
        public readonly DateTime Date;
        public readonly int SubjectId;
        public readonly DayOfWeek Day;

        public TimeLineWidgetSubjectAction(DateTime Date, int SubjectId, DayOfWeek Day)
        {
            this.Date = Date;
            this.SubjectId = SubjectId;
            this.Day = Day;
        }
    }
}
