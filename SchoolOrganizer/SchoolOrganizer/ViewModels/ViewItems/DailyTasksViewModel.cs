using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kit.Model;
using SchoolOrganizer.Models.DailyTasksModels;
using SchoolOrganizer.Views.ViewItems;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Log = Kit.Log;

namespace SchoolOrganizer.ViewModels.ViewItems
{
    public class DailyTasksViewModel : ModelBase
    {

        public ObservableCollection<DailyTasks> LoadedDays { get; }

        private DailyTasks _CurrentDailyTasks;

        public DailyTasks CurrentDailyTasks
        {
            get => _CurrentDailyTasks;
            set
            {
                if (_CurrentDailyTasks != value)
                {
                    var oldDate = CurrentDate;
                    _CurrentDailyTasks = value;
                    Raise(() => CurrentDailyTasks);

                    if (_CurrentDailyTasks.Date > oldDate)
                    {
                        GoForward();
                    }
                    else
                    {
                        GoBackward();
                    }
                }
            }
        }

        private DateTime CurrentDate => CurrentDailyTasks.Date;
        private DateTime LastSwipeStamp;
        public DailyTasksViewModel()
        {
            LastSwipeStamp = DateTime.MinValue;
            _CurrentDailyTasks= new DailyTasks(DateTime.Today);
            this.LoadedDays = new ObservableCollection<DailyTasks>()
            {
                new DailyTasks(DateTime.Today.AddDays(-1)),
                CurrentDailyTasks,
                new DailyTasks(DateTime.Today.AddDays(1))
            };
        }

        public void GoBackward()
        {
            Log.Logger.Debug("Go back");
            int index = LoadedDays.IndexOf(CurrentDailyTasks);

            if (index == 0)
            {
                LoadedDays.Insert(index , new DailyTasks(CurrentDate.AddDays(-1)));
            }
        }

        public void GoForward()
        {
            Log.Logger.Debug("Go forward");
            int index = LoadedDays.IndexOf(CurrentDailyTasks);

            if (index == LoadedDays.Count - 1)
            {
                LoadedDays.Insert(index+1,new DailyTasks(CurrentDate.AddDays(1)));
            }
        }
    }
}
