using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolOrganizer.Widgets
{
    public class WidgetPendingAction
    {
        public readonly string Action;
        public readonly object[] Parameters;
        public WidgetPendingAction(string Action, params object[] Parameters)
        {
            this.Parameters = Parameters;
            this.Action = Action;
        }
    }
}
