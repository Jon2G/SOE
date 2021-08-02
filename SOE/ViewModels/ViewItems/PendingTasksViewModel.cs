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
            Refresh(PendingStatus.Pending).SafeFireAndForget();
        }

        public async Task Refresh(Enums.PendingStatus status= Enums.PendingStatus.Pending)
        {
            await Task.Yield();
            DayGroups.Clear();
            DayGroups.AddRange(
                AppData.Instance.LiteConnection.
                    Lista<long>($"SELECT Distinct {nameof(ToDo.Date)} from {nameof(ToDo)} where {nameof(ToDo.Date)}>={DateTime.Today.Ticks} AND STATUS={(int)status} order by date")
                    .Select((x) => new ByDayGroup()
                    {
                        FDateTime = new DateTime(x)
                    }).ToList());
            foreach (var day in DayGroups)
            {
                day.Refresh(status);
            }
        }

    }
}
