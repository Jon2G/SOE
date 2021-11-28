using System;
using SOEWeb.Shared;
using Kit;
using Kit.Model;
using Kit.Sql.Attributes;
using Kit.Sql.Interfaces;
using SOE.Data;
using System.Windows.Input;
using Xamarin.Forms;
using SOE.Enums;
using SOE.API;
using SOEWeb.Shared.Enums;
using AsyncAwaitBestPractices;
using Kit.Services.Web;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SOE.Models
{
    public class Reminder : ReminderBase
    {


        private PendingStatus _Status;

        public PendingStatus Status
        {
            get => _Status;
            set
            {
                _Status = value;
                Raise(() => Status);
                Raise(() => IsComplete);
            }
        }
        public bool IsComplete => Status == PendingStatus.Done;

        [Ignore]
        public string FormattedTime => $"{this.Time:hh}:{this.Time:mm}";
        [Ignore]
        public string FormattedDate => $"{this.Date.DayOfWeek.GetDayName()} - {this.Date:dd/MM}";
        //public Reminder(string Title, DateTime dateTime)
        //{
        //    this.Title = Title;
        //    this.Date = dateTime;
        //}
        public Reminder()
        {
            this.Date = DateTime.Today;
        }
        public static async Task Save(Reminder reminder)
        {
            await Task.Yield();
            reminder.Date = new DateTime(reminder.Date.Year, reminder.Date.Month, reminder.Date.Day);
            AppData.Instance.LiteConnection.InsertOrReplace(reminder);

        }
        internal static async Task<string> ShareReminder(Reminder reminder)
        {
            await Task.Yield();
            if (reminder.Subject is not null && reminder.Subject.IsOffline)
            {
                if (!await reminder.Subject.Sync(AppData.Instance, new SyncService()))
                {
                    App.Current.MainPage.DisplayAlert("Opps...",
                        "No fue posible compartir este recordatorio, revise su conexión a internet", "Ok").SafeFireAndForget();
                    return null;
                }
            }
            Response Response = await APIService.PostReminder(reminder);
            if (Response.ResponseResult != APIResponseResult.OK)
            {
                App.Current.MainPage.DisplayAlert("Opps...", "No fue posible compartir este recordatorio, revise su conexión a internet", "Ok").SafeFireAndForget();
                return null;
            }
            return DynamicLinkFormatter.GetDynamicUrl("share",
                new Dictionary<string, string>() { { "type", "reminder" }, { "id", reminder.Guid.ToString("N") } });
        }

    }
}
