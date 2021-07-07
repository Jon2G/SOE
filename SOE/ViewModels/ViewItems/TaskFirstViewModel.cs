using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AsyncAwaitBestPractices;
using Kit.Model;
using P42.Utils;
using SOE.Data;
using SOE.Models.TaskFirst;
using SOE.Views.Pages;
using SOE.Views.PopUps;
using Xamarin.Forms;

namespace SOE.ViewModels.ViewItems
{
    public class TaskFirstViewModel : ModelBase
    {
        public static TaskFirstViewModel Instance { get; private set; }
        public ObservableCollection<ByDayGroup> DayGroups { get; set; }
        public const string Done = "AND DONE=1 AND ARCHIVED=0";
        public const string Pending = "AND DONE=0 AND ARCHIVED=0";
        public const string Archived = "AND ARCHIVED=1";
        
        public TaskFirstViewModel()
        {
            Instance = this;
            DayGroups = new ObservableCollection<ByDayGroup>();
            Refresh(Pending).SafeFireAndForget();
        }

        public async Task Refresh(string condition = "")
        {
            await Task.Yield();
            DayGroups.Clear();
            DayGroups.AddRange(
                AppData.Instance.LiteConnection.
                    Lista<long>($"SELECT Distinct {nameof(ToDo.Date)} from {nameof(ToDo)} where {nameof(ToDo.Date)}>={DateTime.Today.Ticks} {condition} order by date")
                    .Select((x) => new ByDayGroup()
                    {
                        FDateTime = new DateTime(x)
                    }).ToList());
            foreach (var day in DayGroups)
            {
                day.Refresh(condition);
            }
        }

    }
}
