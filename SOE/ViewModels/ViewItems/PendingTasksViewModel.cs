﻿using AsyncAwaitBestPractices;
using Kit;
using Kit.Model;
using SOE.Enums;
using SOE.Models.TodoModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;


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

        private async Task _Refresh(Action OnRefreshComplete, PendingStatus status)
        {
            try
            {
                List<ToDo> todos = await ToDo.Get(status);
                Dictionary<DateTime, ByDayGroup> groups = new Dictionary<DateTime, ByDayGroup>();
                List<BySubjectGroup> subjectGroups = null;
                foreach (ToDo toDo in todos)
                {
                    await toDo.GetSubject();
                    if (!groups.TryGetValue(toDo.Date, out ByDayGroup group))
                    {
                        subjectGroups = new List<BySubjectGroup>();
                        group = new ByDayGroup(toDo.Date);
                        groups.Add(toDo.Date, group);
                    }
                    group.Add(toDo, subjectGroups);
                    group.SubjectGroups = new ObservableCollection<BySubjectGroup>(subjectGroups);
                }
                Device.BeginInvokeOnMainThread(() =>
                {
                    DayGroups.Clear();
                    DayGroups.AddRange(groups.Values);
                    this.OnPropertyChanged(nameof(this.DayGroups));
                    OnRefreshComplete?.Invoke();
                });
            }
            catch (Exception e)
            {
                if (Tools.Debugging)
                {
                    throw e;
                }
                Log.Logger.Error(e, "Refresh pending tasks viewmodel");

            }
        }
        public void Refresh(Action OnRefreshComplete, PendingStatus status = PendingStatus.Pending)
        {
            _Refresh(OnRefreshComplete, status).SafeFireAndForget();
        }

    }
}
