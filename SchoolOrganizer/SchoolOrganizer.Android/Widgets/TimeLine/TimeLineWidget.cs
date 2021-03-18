using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SchoolOrganizer.Models.Scheduler;
using Kit;

namespace SchoolOrganizer.Droid.Widgets
{
    public static class TimeLineWidget
    {
        public const string TOAST_ACTION = "com.example.widgets.WidgetProviders.TOAST_ACTION";
        public const string FOWARD_ACTION = "FOWARD_ACTION";
        public const string BACKWARD_ACTION = "BACKWARD_ACTION";
        public const string DAY_CHANGED = "DAY_CHANGED";
        public const string EXTRA_ITEM = "com.example.widgets.WidgetProviders.EXTRA_ITEM";

        private static readonly Dictionary<int, Day> WidgetsDays;

        static TimeLineWidget()
        {
            WidgetsDays = new Dictionary<int, Day>();
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