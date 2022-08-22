using Kit.Forms.Services.Interfaces;
using SOE.Models.TodoModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SOE.Widgets
{
    public class ToDosWidget : IWidget
    {
        public const string AppWidgetProviderFullClass = "SOE.Droid.Widgets.ToDos.ToDosWidgetProvider";
        public override string AppWidgetProviderFullClassName => AppWidgetProviderFullClass;
        public const string ITEM_CLICK = "TODOS_WIDGET_ITEM_CLICK";
        public const string ITEM_INDEX = "com.example.widgets.WidgetProviders.ITEM_INDEX";
        private static readonly Dictionary<int, List<ToDo>> WidgetsTodos;
        static ToDosWidget()
        {
            WidgetsTodos = new Dictionary<int, List<ToDo>>();
        }

        public static async Task<List<ToDo>> Refresh(int WidgetId)
        {
            await Task.Yield();
            if (!WidgetsTodos.ContainsKey(WidgetId))
            {
                return WidgetsTodos[WidgetId] = await GetTasks(WidgetId);
            }
            return WidgetsTodos[WidgetId] = await GetTasks();
        }

        public static Task<List<ToDo>> GetTasks() => ToDo.Get();

        public static async Task<List<ToDo>> GetTasks(int WidgetId)
        {
            await Task.Yield();
            try
            {
                if (!WidgetsTodos.ContainsKey(WidgetId))
                {
                    return WidgetsTodos[WidgetId] = await GetTasks();
                }

                return WidgetsTodos[WidgetId];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return await Task.FromResult(new List<ToDo>());
        }

        public static void Unload(int WidgetId)
        {
            if (WidgetsTodos.ContainsKey(WidgetId))
                WidgetsTodos.Remove(WidgetId);
        }
        public static ToDo GetItemAt(int appWidgetId, int itemPosition)
        {
            return GetTasks(appWidgetId).GetAwaiter().GetResult()[itemPosition];
        }

        public static string GetColor(ToDo toDo) => GetColor(ToDo.DaysLeft(toDo));
        public static string GetEmoji(ToDo toDo) => GetEmoji(ToDo.DaysLeft(toDo));

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
            IWidget.UpdateWidget(new ToDosWidget());
        }
    }
}