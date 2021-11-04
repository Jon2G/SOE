using SOE.API;
using SOE.FireBase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SOE.Services.ActionResponse
{
    public class ShareReminderAction : IActionResponse
    {
        private readonly Guid Id;

        public ShareReminderAction(Guid Id)
        {
            this.Id = Id;
        }

        public async Task Execute()
        {
            await Task.Yield();
            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Descargando recordatorio..."))
            {
                await APIService.DownloadSharedReminder(this.Id);
            }
        }
    }
}
