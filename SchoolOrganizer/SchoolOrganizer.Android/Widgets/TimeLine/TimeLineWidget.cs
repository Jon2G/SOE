using System.Collections.Generic;
using Kit;
using SchoolOrganizer.Models.Scheduler;

namespace SchoolOrganizer.Droid.Widgets.TimeLine
{
    public static class TimeLineWidget
    {
        public const string FOWARD_ACTION = "FOWARD_ACTION";
        public const string BACKWARD_ACTION = "BACKWARD_ACTION";
        public const string EXTRA_ITEM = "com.example.widgets.WidgetProviders.EXTRA_ITEM";
        public const string CLICK = "CLICK";

        private static readonly Dictionary<int, Day> WidgetsDays;

        static TimeLineWidget()
        {
            WidgetsDays = new Dictionary<int, Day>();
        }
        public static void Today(int WidgetId)
        {
            Init(WidgetId);
            Day day = Day.Today();
            WidgetsDays[WidgetId] = day;
            Log.Logger.Debug("Today ->{0}", day.DayOfWeek);
        }
        public static void Tomorrow(int WidgetId)
        {
            Init(WidgetId);
            Day day = WidgetsDays[WidgetId].Tommorrow();
            WidgetsDays[WidgetId] = day;
            Log.Logger.Debug("Tomorrow ->{0}", day.DayOfWeek);
        }

        public static void Yesterday(int WidgetId)
        {
            Init(WidgetId);
            Day day = WidgetsDays[WidgetId].Yesterday();
            WidgetsDays[WidgetId] = day;
            Log.Logger.Debug("Yesterday ->{0}", day.DayOfWeek);
        }

        internal static List<ClassSquare> GetTimeLine(int WidgetId)
        {
            Init(WidgetId);
            Day day = WidgetsDays[WidgetId];
            Log.Logger.Debug("GetTimeLine of day {0}", day.DayOfWeek);
            return day.GetTimeLine();
        }

        private static void Init(int WidgetId)
        {
            if (!WidgetsDays.ContainsKey(WidgetId))
                WidgetsDays.Add(WidgetId, Day.Today());
        }

        public static void Unload(int WidgetId)
        {
            if (WidgetsDays.ContainsKey(WidgetId))
                WidgetsDays.Remove(WidgetId);
        }

        internal static Day GetDay(int WidgetId)
        {
            Init(WidgetId);
            Day day = WidgetsDays[WidgetId];
            Log.Logger.Debug("GetDay => {0}", day.DayOfWeek);
            return day;
        }
    }
}