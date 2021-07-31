﻿using AsyncAwaitBestPractices;
using Kit;
using Kit.Model;
using SOE.Data;
using SOE.Models;
using SOE.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SOE.ViewModels.ViewItems
{
    public class PendingRemindersViewModel : ModelBase
    {
        public static PendingRemindersViewModel Instance { get; private set; }
        private ObservableCollection<Reminder> _Reminders;
        public ObservableCollection<Reminder> Reminders
        {
            get => _Reminders;
            set
            {
                _Reminders = value;
                Raise(() => Reminders);
            }
        }
        private Reminder _PReminder;

        public Reminder PReminder
        {
            get => _PReminder;
            set
            {
                _PReminder = value;
                Raise(() => PReminder);
            }
        }
        public PendingRemindersViewModel()
        {
            Instance = this;
            Reminders = new ObservableCollection<Reminder>();
            Load().SafeFireAndForget();

        }

        public async Task Load()
        {
            await Task.Yield();
            Reminders.AddRange(AppData.Instance.LiteConnection.Table<Reminder>().ToList());
            foreach (var reminder in Reminders)
            {
                if (reminder.SubjectId > 0)
                {
                    reminder.Subject = SubjectService.Get(reminder.SubjectId);
                }
            }

        }
    }
}
