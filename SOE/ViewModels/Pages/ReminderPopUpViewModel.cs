using AsyncAwaitBestPractices;
using SOE.Data;
using SOE.Enums;
using SOE.Fonts;
using SOE.Models;
using SOE.ViewModels.ViewItems;
using SOE.Views.Pages;
using SOE.Views.PopUps;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SOE.ViewModels
{
    public class ReminderPopUpViewModel
    {
        private readonly ReminderPopUp PopUp;
        private readonly Reminder reminder;
        public ReminderPopUpViewModel(ReminderPopUp popUp, Reminder Reminder)
        {
            this.PopUp = popUp;
            this.reminder = Reminder;
        }
        private ICommand _TappedCommand;
        public ICommand TappedCommand => _TappedCommand ??= new Command<string>(Tapped);


        public string ArchiveText => reminder.Status.HasFlag(PendingStatus.Archived) ? "Desarchivar" : "Archivar";
        public FontImageSource IconTwo
        {
            get
            {
                var IconTwo = new FontImageSource()
                {
                    FontFamily = FontelloIcons.Font,
                    Glyph = reminder.Status.HasFlag(PendingStatus.Archived) ? FontelloIcons.Folder : FontelloIcons.Archive
                };
                IconTwo.SetOnAppTheme(FontImageSource.ColorProperty, Color.Black, Color.White);
                return IconTwo;
            }
        }
        private async void Tapped(string Action)
        {
            PopUp.Close().SafeFireAndForget();
            switch (Action)
            {
                case "Editar":
                    OpenTask();
                    break;
                case "Eliminar":
                    Eliminar();
                    break;
                case "Archivar":
                    if (reminder.Status.HasFlag(PendingStatus.Archived))
                    {
                        Desarchivar();
                        break;
                    }
                    Archivar();
                    break;
                case "Compartir":
                    if (DateTime.Now > reminder.Date)
                    {
                        App.Current.MainPage.DisplayAlert(reminder.Title,
                            "Este recordatorio ya ha expirado, cambie la fecha de entrega si desea compartirla", "Ok.")
                            .SafeFireAndForget();
                        return;
                    }
                    string link = await Models.Reminder.ShareReminder(reminder);
                    if (!string.IsNullOrEmpty(link))
                    {
                        Share.RequestAsync(link, "Compartir recordatorio").SafeFireAndForget();
                    }
                    return;
            }

        }
        private async void OpenTask()
        {
            ReminderPage pr = new ReminderPage(this.reminder);
            await pr.ShowDialog();
        }
        public async void Eliminar()
        {
            AppData.Instance.LiteConnection.Delete(this.reminder);
            PendingRemindersViewModel.Instance.Reminders.Remove(this.reminder);
            await PendingRemindersViewModel.Instance.Load();
        }
        private void Archivar()
        {
            this.reminder.Status |= PendingStatus.Archived;
            AppData.Instance.LiteConnection.Update(this.reminder);
        }
        
        private void Desarchivar()
        {
            this.reminder.Status -= PendingStatus.Archived;
            AppData.Instance.LiteConnection.Update(this.reminder);
        }
       
    }
}
