using AsyncAwaitBestPractices;
using SOE.Enums;
using SOE.Fonts;
using SOE.Models;
using SOE.ViewModels.ViewItems;
using SOE.Views.Pages;
using SOE.Views.PopUps;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SOE.ViewModels
{
    public class ReminderPopUpViewModel
    {
        private readonly ReminderPopUp PopUp;
        private readonly Reminder Reminder;
        public ReminderPopUpViewModel(ReminderPopUp popUp, Reminder Reminder)
        {
            this.PopUp = popUp;
            this.Reminder = Reminder;
        }
        private ICommand _TappedCommand;
        public ICommand TappedCommand => _TappedCommand ??= new Command<string>(Tapped);


        public string ArchiveText => Reminder.Status.HasFlag(PendingStatus.Archived) ? "Desarchivar" : "Archivar";
        public FontImageSource IconTwo
        {
            get
            {
                FontImageSource? IconTwo = new FontImageSource()
                {
                    FontFamily = FontelloIcons.Font,
                    Glyph = Reminder.Status.HasFlag(PendingStatus.Archived) ? FontelloIcons.Folder : FontelloIcons.Archive
                };
                IconTwo.SetOnAppTheme(FontImageSource.ColorProperty, Color.Black, Color.White);
                return IconTwo;
            }
        }
        private void Tapped(string Action)
        {
            PopUp.Close().SafeFireAndForget();
            switch (Action)
            {
                case "Editar":
                    OpenTask().SafeFireAndForget();
                    break;
                case "Eliminar":
                    Eliminar().SafeFireAndForget();
                    break;
                case "Archivar":
                    if (Reminder.Status.HasFlag(PendingStatus.Archived))
                    {
                        Desarchivar().SafeFireAndForget();
                        break;
                    }
                    Archivar().SafeFireAndForget();
                    break;
                case "Compartir":
                    if (DateTime.Now > Reminder.Date)
                    {
                        App.Current.MainPage.DisplayAlert(Reminder.Title,
                            "Este recordatorio ya ha expirado, cambie la fecha de entrega si desea compartirla", "Ok.")
                            .SafeFireAndForget();
                        return;
                    }

                    Models.Reminder.ShareReminder(Reminder).ContinueWith(t =>
                    {
                        string link = t.Result;
                        if (!string.IsNullOrEmpty(link))
                        {
                            Share.RequestAsync(link, "Compartir recordatorio").SafeFireAndForget();
                        }
                    }).SafeFireAndForget();
                    return;
            }

        }
        private Task OpenTask()
        {
            ReminderPage pr = new ReminderPage(this.Reminder);
            return pr.ShowDialog();
        }
        public async Task Eliminar()
        {
            await this.Reminder.Delete();
            PendingRemindersViewModel.Instance.Reminders.Remove(this.Reminder);
            PendingRemindersViewModel.Instance.Load();
        }
        private Task Archivar()
        {
            this.Reminder.Status |= PendingStatus.Archived;
            return this.Reminder.Save();
        }

        private Task Desarchivar()
        {
            this.Reminder.Status -= PendingStatus.Archived;
            return this.Reminder.Save();
        }
    }
}
