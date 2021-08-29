using System;
using System.Linq;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using Kit;
using Kit.Forms.Services.Interfaces;
using SOE.API;
using SOE.Data;
using SOE.Interfaces;
using SOE.Models.Scheduler;
using SOE.Models.TaskFirst;
using SOE.Services;
using SOE.Services.ActionResponse;
using SOE.Views.ViewItems.ScheduleView;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using DeviceInfo = Xamarin.Forms.Internals.DeviceInfo;
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
            this.OnAppearingAsync().SafeFireAndForget();
        }

        private async Task OnAppearingAsync()
        {
            await Task.Yield();
            if (this.Model.SelectedIndex <= 0)
            {
                Dispatcher.BeginInvokeOnMainThread(() =>
                {
                    this.Model.SelectedIndex = 1;
                });
            }
            await this.Model.OnAppearing();
            DependencyService.Get<IStartNotificationsService>()?.StartNotificationsService();
            if (Device.RuntimePlatform == Device.iOS)
            {
                var appTrackingTransparencyPermission = DependencyService.Get<IAppTrackingTransparencyPermission>();
                var status = await appTrackingTransparencyPermission.CheckStatusAsync();
                switch (status)
                {
                    case PermissionStatus.Denied:
                    case PermissionStatus.Granted:
                        return;
                    case PermissionStatus.Disabled:
                    case PermissionStatus.Unknown:
                        appTrackingTransparencyPermission.RequestAsync((s) => { });
                        break;
                }
            }
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