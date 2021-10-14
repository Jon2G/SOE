using System;
using System.Collections.Generic;
using Kit.Forms.Services.Interfaces;
using SOE.Models.Scheduler;
using Xamarin.Forms;

namespace SOE.Widgets
{
    public class TimeLineWidget : IWidget
    {
        public const string AppWidgetProviderFullClass = "SOE.Droid.Widgets.TimeLine.TimeLineWidgetProvider";
        public override string AppWidgetProviderFullClassName =>AppWidgetProviderFullClass;
        public const string FOWARD_ACTION = "TIMELINE_FOWARD_ACTION";
        public const string BACKWARD_ACTION = "TIMELINE_BACKWARD_ACTION";
        public const string EXTRA_ITEM = "com.example.widgets.WidgetProviders.EXTRA_ITEM";
        public const string ITEM_CLICK = "TIMELINE_WIDGET_ITEM_CLICK";
        public const string DAY_CLICK = "TIMELINE_WIDGET_DAY_CLICK";

        private static readonly Dictionary<int, Day> WidgetsDays;
        private static readonly Dictionary<DayOfWeek, List<ClassSquare>> SavedTimeLines;

        static TimeLineWidget()
        {
            WidgetsDays = new Dictionary<int, Day>();
            SavedTimeLines = new Dictionary<DayOfWeek, List<ClassSquare>>();
        }
        public static void Today(int WidgetId)
        {
            Init(WidgetId);
            Day day = Day.Today();
            WidgetsDays[WidgetId] = day;
            //Log.Logger.Debug("Today ->{0}", day.DayOfWeek);
        }
        public static void Tomorrow(int WidgetId)
        {
            Init(WidgetId);
            Day day = WidgetsDays[WidgetId].Tommorrow();
            WidgetsDays[WidgetId] = day;
            //Log.Logger.Debug("Tomorrow ->{0}", day.DayOfWeek);
        }

        public static void Yesterday(int WidgetId)
        {
            Init(WidgetId);
            Day day = WidgetsDays[WidgetId].Yesterday();
            WidgetsDays[WidgetId] = day;
            //Log.Logger.Debug("Yesterday ->{0}", day.DayOfWeek);
        }

        public static List<ClassSquare> GetTimeLine(int WidgetId)
        {
            Day day = GetDay(WidgetId);
            if (!SavedTimeLines.ContainsKey(day.DayOfWeek))
            {
                Init(WidgetId);
                //Log.Logger.Debug("GetTimeLine of day {0}", day.DayOfWeek);
                SavedTimeLines.Add(day.DayOfWeek, day.GetTimeLine());
                return GetTimeLine(WidgetId);
            }
            return SavedTimeLines[day.DayOfWeek];
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

        public static Day GetDay(int WidgetId)
        {
            Init(WidgetId);
            Day day = WidgetsDays[WidgetId];
            //Log.Logger.Debug("GetDay => {0}", day.DayOfWeek);
            return day;
        }

        public static ClassSquare GetItemAt(int appWidgetId, int itemPosition)
        {
            return GetTimeLine(appWidgetId)[itemPosition];
        }
        public static Day Refresh(int WidgetId)
        {
            if (!WidgetsDays.ContainsKey(WidgetId))
            {
                return WidgetsDays[WidgetId] = GetDay(WidgetId);
            }
            return WidgetsDays[WidgetId] = GetDay(WidgetId);
        }
        public static void UpdateWidget()
        {
            IWidget.UpdateWidget(new TimeLineWidget());
        }
    }
}