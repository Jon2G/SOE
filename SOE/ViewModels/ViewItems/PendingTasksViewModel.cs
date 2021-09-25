using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using Kit.Daemon.Devices;
using Kit.Model;
using P42.Utils;
using SOE.Data;
using SOE.Enums;
using SOE.Models.TaskFirst;
using SOE.Views.ViewItems;
using SOE.Views.ViewItems.TasksViews;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections;

namespace SOE.ViewModels.ViewItems
{
    public class PendingTasksViewModel : ModelBase
    {
        public static PendingTasksViewModel Instance { get; private set; }
        private readonly object _lock = new object();

        private ObservableCollection<ByDayGroup> _DayGroups;

        public ObservableCollection<ByDayGroup> DayGroups
        {
            get => this._DayGroups;
            private set
            {
                this._DayGroups = value;
                Raise(() => DayGroups);
            }
        }

        public PendingTasksViewModel()
        {
            Instance = this;
            _DayGroups = new ObservableCollection<ByDayGroup>();
            Binding.EnableCollectionSynchronization(DayGroups, _lock, Callback);
        }

        private void Callback(IEnumerable collection, object context, Action accessMethod, bool writeAccess)
        {
            lock (collection)
            {
                accessMethod?.Invoke();
            }
        }

        public void Refresh(Action OnRefreshComplete, PendingStatus status = PendingStatus.Pending)
        {
            DayGroups.Clear();
            List<long> dates =
                AppData.Instance.LiteConnection.Lista<long>($"SELECT Distinct {nameof(ToDo.Date)} from {nameof(ToDo)} where  STATUS={(int)status} order by date");
            foreach (long ldate in dates)
            {
                var date = new DateTime(ldate);
                var day = new ByDayGroup(date);
                day.Refresh(status);
                DayGroups.Add(day);
            }
            this.OnPropertyChanged(nameof(this.DayGroups));
            OnRefreshComplete?.Invoke();
        }

    }
}
