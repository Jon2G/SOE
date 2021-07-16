using AsyncAwaitBestPractices;
using Kit.Model;
using SOE.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace SOE.ViewModels.ViewItems
{
    public class PendingRemindersViewModel : ModelBase
    {
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
        public PendingRemindersViewModel()
        {
            Reminders = new ObservableCollection<Reminder>();
            Load().SafeFireAndForget();
        }
        private async Task Load()
        {
            await Task.Yield();
            for (int i = 0; i < 10; i++)
            {
                Reminders.Add(new Reminder("Preguntar sobre evaluación", DateTime.Now));
            }
        }
    }
}
