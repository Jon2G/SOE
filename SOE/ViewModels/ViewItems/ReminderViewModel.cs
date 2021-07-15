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
    public class ReminderViewModel : ModelBase
    {
        private ObservableCollection<Reminders> _Reminders;
        public ObservableCollection<Reminders> Reminders
        {
            get => _Reminders;
            set
            {
                _Reminders = value;
                Raise(() => Reminders);
            }
        }
        public ReminderViewModel()
        {
            Reminders = new ObservableCollection<Reminders>();
            Load().SafeFireAndForget();
        }
        private async Task Load()
        {
            await Task.Yield();
            for (int i = 0; i < 10; i++)
            {
                Reminders.Add(new Reminders("Preguntar sobre evaluación", DateTime.Now));
            }
        }
    }
}
