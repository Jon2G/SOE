using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AsyncAwaitBestPractices;
using Kit.Model;
using P42.Utils;
using SOE.Data;
using SOE.Enums;
using SOE.Models.TaskFirst;
using SOE.Views.Pages;
using SOE.Views.PopUps;
using Xamarin.Forms;

namespace SOE.ViewModels.ViewItems
{
    public class PendingTasksViewModel : ModelBase
    {
        public static PendingTasksViewModel Instance { get; private set; }
        public ObservableCollection<ByDayGroup> DayGroups { get; set; }
        public ToDoStatus Status { get; set; }
        public PendingTasksViewModel()
        {
            this.Status = ToDoStatus.Pending;
            Instance = this;
            DayGroups = new ObservableCollection<ByDayGroup>();
            Refresh(ToDoStatus.Pending).SafeFireAndForget();
        }

        public async Task Refresh(ToDoStatus status=ToDoStatus.Invalido)
        {
            if (status == ToDoStatus.Invalido)
            {
                status = this.Status;
            }
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
