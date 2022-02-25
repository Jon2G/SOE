using AsyncAwaitBestPractices;
using Kit;
using SOE.FireBase;
using SOE.Models;
using SOE.Models.TodoModels;
using SOE.Views.Pages;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SOE.Services.ActionResponse
{
    public class TimeLineWidgetSubjectAction : IActionResponse
    {
        public readonly DateTime Date;
        public readonly string SubjectId;
        public readonly DayOfWeek Day;

        public TimeLineWidgetSubjectAction(DateTime Date, string SubjectId, DayOfWeek Day)
        {
            this.Date = Date;
            this.SubjectId = SubjectId;
            this.Day = Day;
        }

        public async Task Execute()
        {
            await Task.Yield();
            ToDo Tarea = new ToDo
            {
                Date = Day.GetNearest(),
                Subject = await Subject.Get(SubjectId),
                Time = Date.TimeOfDay
            };
            await Task.Run(() => { while (Shell.Current is null) { } });
            MasterPage.Instance.Model.SelectedIndex = 2;
            Shell.Current.Navigation.PushAsync(new NewTaskPage(Tarea)).SafeFireAndForget();
        }
    }
}
