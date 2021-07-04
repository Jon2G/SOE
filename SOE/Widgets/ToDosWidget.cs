using System;
using System.Collections.Generic;
using System.Linq;
using Kit.Forms.Services.Interfaces;
using SOE.Data;
using SOE.Models.Scheduler;
using SOE.Models.TaskFirst;
using SOE.ViewModels.ViewItems;
using SOE.Views.ViewItems.TasksViews;
using Xamarin.Forms;

namespace SOE.Widgets
{
    public static class ToDosWidget
    {
        public const string ITEM_CLICK = "TODOS_WIDGET_ITEM_CLICK"; 
        public const string EXTRA_ITEM = "com.example.widgets.WidgetProviders.EXTRA_ITEM";
        private static readonly Dictionary<int, List<ToDo>> WidgetsTodos;
        static ToDosWidget()
        {
            WidgetsTodos = new Dictionary<int, List<ToDo>>();
        }

        public static List<ToDo> Refresh(int WidgetId)
        {
            if (!WidgetsTodos.ContainsKey(WidgetId))
            {
                return WidgetsTodos[WidgetId] = GetTasks(WidgetId);
            }
            return WidgetsTodos[WidgetId] = GetTasks();
        }

        private static List<ToDo> GetTasks()=> AppData.Instance.LiteConnection.DeferredQuery<ToDo>
                ($"SELECT * from {nameof(ToDo)} where DONE=0 AND ARCHIVED=0 order by Date,Time,SubjectId")
            .Select(x => x.LoadSubject()).ToList();

        public static List<ToDo> GetTasks(int WidgetId)
        {
            if (!WidgetsTodos.ContainsKey(WidgetId))
            {
                return WidgetsTodos[WidgetId] = GetTasks();
            }
            return WidgetsTodos[WidgetId];
        }

        public static void Unload(int WidgetId)
        {
            if (WidgetsTodos.ContainsKey(WidgetId))
                WidgetsTodos.Remove(WidgetId);
        }
        public static ToDo GetItemAt(int appWidgetId, int itemPosition)
        {
            return GetTasks(appWidgetId)[itemPosition];
        }
        private static int DaysLeft(ToDo toDo) => DaysLeft(toDo.Date.Add(toDo.Time));
        private static int DaysLeft(DateTime date) => (date - DateTime.Now).Days;
        public static string GetColor(ToDo toDo) => GetColor(DaysLeft(toDo));
        public static string GetEmoji(ToDo toDo) => GetEmoji(DaysLeft(toDo));

        public static string GetEmoji(int DaysLeft)
        {
            switch (DaysLeft)
            {
                case 0:
                    return "😳";
                case 1:
                    return "👀";
                case 2:
                    return "🧐";
                case 3:
                    return "🤨";
                case 4:
                    return "🙂";
                case 5:
                    return "😉";
                case 6:
                    return "😌";
                case 7:
                    return "😎";

                default:
                    return "⏰";
            }
        }


        public static string GetColor(int DaysLeft)
        {
            switch (DaysLeft)
            {
                case 0:
                    return "#d46a6a";
                case 1:
                    return "#e89d5f";
                case 2:
                    return "#fcc197";
                case 3:
                    return "#e3c16f";
                case 4:
                    return "#fffccc";
                case 5:
                    return "#c7ffe2";
                case 6:
                    return "#abffd3";
                case 7:
                    return "#74d6a2";

                default:
                    return Color.LightBlue.ToHex();
            }
        }
        public static void UpdateWidget()
        {
            IUpdateWidget iWidget = DependencyService.Get<IUpdateWidget>();
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    iWidget?.UpdateWidget("SOE.Droid", "SOE.Droid.Widgets.ToDos.ToDosWidgetProvider");
                    break;
                case Device.iOS:
                    break;
            }
        }
    }
}