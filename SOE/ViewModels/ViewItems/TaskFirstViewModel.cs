using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Kit.Model;
using P42.Utils;
using SOE.Data;
using SOE.Models.TaskFirst;

namespace SOE.ViewModels.ViewItems
{
    public class TaskFirstViewModel : ModelBase
    {
        public ObservableCollection<ByDayGroup> DayGroups { get; set; }
       
        public TaskFirstViewModel()
        {

            DayGroups = new ObservableCollection<ByDayGroup>();
            Task.Run(Refresh);
           
        }

      

        public async Task Refresh()
        {
            await Task.Yield();
            DayGroups.Clear();
            DayGroups.AddRange(
            AppData.Instance.LiteConnection.
                Lista<long>($"SELECT Distinct {nameof(ToDo.Date)} from {nameof(ToDo)} where {nameof(ToDo.Date)}>={DateTime.Today.Ticks} order by date")
                .Select((x) => new ByDayGroup()
                {
                    FDateTime =new DateTime(x)
                }).ToList());
            foreach (var day in DayGroups)
            {
                day.Refresh();
            }
        }

    }
}
