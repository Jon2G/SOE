using AsyncAwaitBestPractices;
using SOE.FireBase;
using SOE.Models.TodoModels;
using SOE.Views.Pages;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SOE.Services.ActionResponse
{
    public class TodoWidgetAction : IActionResponse
    {
        public readonly string DocumentId;
        public TodoWidgetAction(string documentId)
        {
            this.DocumentId = documentId;
        }

        public async Task Execute()
        {
            await Task.Yield();
            ToDo todo = await ToDo.Get(this.DocumentId);
            if (todo is null) { return; }
            TaskDetails task = new TaskDetails(todo);
            await Task.Run(() => { while (Shell.Current is null) { } });
            Shell.Current.Navigation.PushAsync(task, false).SafeFireAndForget();
        }
    }
}
