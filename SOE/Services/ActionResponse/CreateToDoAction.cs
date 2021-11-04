using AsyncAwaitBestPractices;
using SOE.FireBase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SOE.Services.ActionResponse
{
    public class CreateToDoAction : IActionResponse
    {
        public string Description { get; set; }
        public CreateToDoAction(string description)
        {
            this.Description = description;
        }
        public async Task Execute()
        {
            await Task.Yield();
            AppShell.Current.DisplayAlert("Ok Google", $"Estas intentando crear una tarea desde el asistente. D-${Description} \n Esta funcionalidad esta en progreso, no le digas a nadie. 🤭🤫", "Ok")
                .SafeFireAndForget();

        }
    }
}
