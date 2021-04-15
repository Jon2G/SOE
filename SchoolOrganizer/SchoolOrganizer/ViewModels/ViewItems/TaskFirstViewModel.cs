using SchoolOrganizer.Data;
using SchoolOrganizer.Models.Scheduler;
using SchoolOrganizer.Models.TaskFirst;
using SchoolOrganizer.ViewModels.Pages;
using SchoolOrganizer.Views.Pages;
using SchoolOrganizer.Views.PopUps;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using P42.Utils;
using Xamarin.Forms;

namespace SchoolOrganizer.ViewModels.ViewItems
{
    public class TaskFirstViewModel : BaseViewModel
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
            AppData.Instance.LiteConnection.DeferredQuery<ToDo>($"SELECT Distinct {nameof(ToDo.Date)} from {nameof(ToDo)} where {nameof(ToDo.Date)}>=? order by date",DateTime.Now)
                .Select((x) => new ByDayGroup()
                {
                    FDateTime = x.Date
                }).ToList());
            foreach (var day in DayGroups)
            {
                day.Refresh();
            }
        }

    }
}
