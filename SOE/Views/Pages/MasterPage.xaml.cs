using System;
using System.Linq;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using Kit;
using SOE.API;
using SOE.Data;
using SOE.Interfaces;
using SOE.Models.Scheduler;
using SOE.Models.TaskFirst;
using SOE.Services;
using SOE.Services.ActionResponse;
using SOE.Views.ViewItems.ScheduleView;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using Log = Kit.Log;

namespace SOE.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPage
    {
        public static MasterPage Instance { get; private set; }
        public MasterPage()
        {
            Instance = this;
            InitializeComponent();
        }
        protected override bool OnBackButtonPressed()
        {
            switch (Model.SelectedView)
            {
                case ScheduleViewMain schedule:
                    if (schedule.OnBackButtonPressed())
                    {
                        return true;
                    }
                    break;
            }
            return base.OnBackButtonPressed();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (this.Model.SelectedIndex <= 0)
            {
                Dispatcher.BeginInvokeOnMainThread(() =>
               {
                   this.Model.SelectedIndex = 1;
                   //Shell.SetNavBarIsVisible(this, this.Model.SelectedIndex != 1);
               });
            }
            DependencyService.Get<IStartNotificationsService>()?.StartNotificationsService();
        }
        public static void ResponseTo(PendingAction PendingAction) => App.Current.Dispatcher.BeginInvokeOnMainThread(action: () => Execute(PendingAction).SafeFireAndForget());
        private static async Task Execute(PendingAction pendingAction)
        {
            await Task.Yield();
            try
            {
                switch (pendingAction)
                {
                    case TimeLineWidgetDayAction dayAction:
                        await Task.Run(() => { while (ScheduleViewMain.Instance is null) { } });
                        MasterPage.Instance.Model.SelectedIndex = 2;
                        SheduleDay sheduleDay = new SheduleDay(Day.GetNearest(dayAction.Day));
                        ScheduleViewMain.Instance.OnDayTappedCommand.Execute(sheduleDay);
                        break;
                    case TimeLineWidgetSubjectAction subjectAction:
                        var Tarea = new ToDo
                        {
                            Date = subjectAction.Day.GetNearest(),
                            Subject = SubjectService.Get(subjectAction.SubjectId),
                            Time = subjectAction.Date.TimeOfDay
                        };
                        await Task.Run(() => { while (Shell.Current is null) { } });
                        MasterPage.Instance.Model.SelectedIndex = 2;
                        Shell.Current.Navigation.PushAsync(new NewTaskPage(Tarea)).SafeFireAndForget();
                        break;

                    case TodoWidgetAction todoAction:
                        AppData.Instance.LiteConnection.CreateTable<ToDo>();
                        ToDo todo = ToDo.GetById(todoAction.Id);
                        if (todo is null) { break; }
                        TaskDetails task = new TaskDetails(todo);
                        await Task.Run(() => { while (Shell.Current is null) { } });
                        Shell.Current.Navigation.PushAsync(task, false).SafeFireAndForget();
                        break;
                    case UrlAction urlAction:
                        string[] segments = urlAction.Url.Segments;
                        int indexOfAction = segments.IndexOf(x => x.Contains(nameof(APIService.ShareTodo)));
                        if (indexOfAction <= 0)
                        {
                            indexOfAction = segments.IndexOf(x => x.Contains(nameof(APIService.ShareReminder)));
                        }

                        if (indexOfAction <= 0 && segments.Length >= indexOfAction + 1) { break; }

                        Guid guid = Guid.Parse(segments[indexOfAction + 1]);

                        if (segments[indexOfAction].Contains(APIService.ShareTodo))
                        {
                            bool IncludeFiles = await App.Current.MainPage.DisplayAlert("Descargar tarea",
                                "¿Descargar también las imágenes de esta tarea?", "Sí", "No");

                            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Descargando tarea..."))
                            {
                                await APIService.DownloadSharedTodo(guid, IncludeFiles);
                            }
                        }
                        else if (segments[indexOfAction].Contains(APIService.ShareReminder))
                        {
                            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Descargando recordatorio..."))
                            {
                                await APIService.DownloadSharedReminder(guid);
                            }
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Execute");
                App.Current.MainPage.DisplayAlert("Error", ex.ToString(), "Ok").SafeFireAndForget();
            }
        }
    }
}