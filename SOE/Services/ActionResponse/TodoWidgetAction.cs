using AsyncAwaitBestPractices;
using SOE.Data;
using SOE.FireBase;
using SOE.Models.TodoModels;
using SOE.Views.Pages;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SOE.Services.ActionResponse
{
    public class TodoWidgetAction : IActionResponse
    {
       public readonly Guid Id;
       public TodoWidgetAction(Guid Id)
       {
           this.Id = Id;
       }

        public async Task Execute()
        {
            await Task.Yield();
            AppData.Instance.LiteConnection.CreateTable<ToDo>();
            ToDo todo = ToDo.GetById(Id);
            if (todo is null) { return; }
            TaskDetails task = new TaskDetails(todo);
            await Task.Run(() => { while (Shell.Current is null) { } });
            Shell.Current.Navigation.PushAsync(task, false).SafeFireAndForget();
        }
    }
}
