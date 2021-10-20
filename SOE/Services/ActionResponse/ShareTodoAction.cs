using SOE.API;
using SOE.FireBase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SOE.Services.ActionResponse
{
    public class ShareTodoAction : IActionResponse
    {
        private readonly Guid Id;
        public ShareTodoAction(Guid Id)
        {
            this.Id = Id;
        }

        public async Task Execute()
        {
            await Task.Yield();
            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Descargando tarea..."))
            {
                await APIService.DownloadSharedTodo(this.Id);
            }

        }
    }
}
