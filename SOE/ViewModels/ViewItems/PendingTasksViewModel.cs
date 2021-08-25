using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using Kit.Model;
using P42.Utils;
using SOE.Data;
using SOE.Enums;
using SOE.Models.TaskFirst;
using SOE.Views.ViewItems;
using SOE.Views.ViewItems.TasksViews;
using System.Collections.Generic;

namespace SOE.ViewModels.ViewItems
{
    public class PendingTasksViewModel : ModelBase
    {
        public static PendingTasksViewModel Instance { get; private set; }
        public ObservableCollection<ByDayGroup> DayGroups { get; set; }

        public PendingTasksViewModel()
        {
            Instance = this;
            DayGroups = new ObservableCollection<ByDayGroup>();
        }

        public async Task Refresh(PendingStatus status = PendingStatus.Pending)
        {
            await Task.Yield();
            DayGroups.Clear();
            List<long> dates =
                AppData.Instance.LiteConnection.Lista<long>($"SELECT Distinct {nameof(ToDo.Date)} from {nameof(ToDo)} where  STATUS={(int)status} order by date");
            foreach (long ldate in dates)
            {
                var date = new DateTime(ldate);
                var day = new ByDayGroup(date);
                DayGroups.Add(day);
                day.Refresh(status).SafeFireAndForget();
            }
        }

    }
}
