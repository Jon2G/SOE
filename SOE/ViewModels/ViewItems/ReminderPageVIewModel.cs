using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using Kit.Model;
using SOE.Models;
using SOE.Views.Pages;
using SOE.Views.PopUps;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SOE.ViewModels.ViewItems
{
    public class ReminderPageVIewModel : ModelBase
    {
        ReminderPage ReminderPage;
        public ICommand SubjectCommand { get; }
        public ICommand SaveReminderCommand { get; set; }
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
        public ReminderPageVIewModel(ReminderPage reminderPage)
        {
            this.ReminderPage = reminderPage;
            PReminder = new Reminder();
            SubjectCommand = new Command(SubjectClicked);
            SaveReminderCommand = new AsyncCommand<Reminder>(Save);
        }

        public ReminderPageVIewModel()
        {
        }

        private async Task SelectSubject()
        {
            var pr = new SubjectPopUp();
            await pr.ShowDialog();
            this.PReminder.Subject = pr.Modelo.SelectedSubject;
        }
        private async void SubjectClicked()
        {
            await SelectSubject();
        }

        private async Task Save(Reminder obj)
        {
            //if (PReminder.Subject == null)
            //{
            //    Acr.UserDialogs.UserDialogs.Instance.Alert("Por favor seleccione una materia");
            //    await SelectSubject();
            //    if (PReminder.Subject is not null)
            //        await Save(obj);
            //    return;
            //}
            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Guardando recordatorio..."))
            {
                await Reminder.Save(this.PReminder);
                ReminderPage.Close().SafeFireAndForget();
                Device.BeginInvokeOnMainThread(() =>
                {
                    PendingRemindersViewModel.Instance.Reminders.Clear();
                    PendingRemindersViewModel.Instance.Load();
                });
            }
        }
    }
}
