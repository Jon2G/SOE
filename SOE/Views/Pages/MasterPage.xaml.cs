using System;
using System.Linq;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using Kit;
using SOE.API;
using SOE.Data;
using SOE.Interfaces;
using SOE.Models.TaskFirst;
using SOE.Services;
using SOE.Services.ActionResponse;
using SOE.Views.ViewItems.ScheduleView;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

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
            if (Device.RuntimePlatform != Device.iOS && TabView.SelectedIndex <= 0)
            {
                TabView.SelectedIndex = 1;
            }
            else if (TabView.SelectedIndex <= 0)
            {
                Dispatcher.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(100);
                    TabView.SelectedIndex = 1;
                });
            }
            Shell.SetNavBarIsVisible(this, this.TabView.SelectedIndex != 1);
            DependencyService.Get<IStartNotificationsService>()?.StartNotificationsService();
        }
        public static void ResponseTo(PendingAction PendingAction) => App.Current.Dispatcher.BeginInvokeOnMainThread(action: () => Execute(PendingAction));
        private static async void Execute(PendingAction pendingAction)
        {
            switch (pendingAction)
            {
                case TimeLineWidgetDayAction dayAction:
                    MasterPage.Instance.TabView.SelectedIndex = 2;
                    ViewItems.ScheduleView.ScheduleViewMain.Instance
                        .Model.WeekDays.FirstOrDefault(x => x.Day.DayOfWeek == dayAction.Day);
                    ViewItems.ScheduleView.ScheduleViewMain.Instance.OnDayTappedCommand?.Execute(dayAction.Day);
                    break;
                case TimeLineWidgetSubjectAction subjectAction:
                    MasterPage.Instance.TabView.SelectedIndex = 2;
                    var Tarea = new ToDo
                    {
                        Date = subjectAction.Day.GetNearest(),
                        Subject = SubjectService.GetByGroup(subjectAction.Group),
                        Time = subjectAction.Date.TimeOfDay
                    };
                    Shell.Current.Navigation.PushAsync(new NewTaskPage(Tarea)).SafeFireAndForget();
                    break;
                case TodoWidgetAction todoAction:
                    AppData.Instance.LiteConnection.CreateTable<ToDo>();
                    ToDo todo = ToDo.GetById(todoAction.Id);
                    if (todo is null) { break; }
                    TaskDetails task = new TaskDetails(todo);
                    Shell.Current.Navigation.PushAsync(task, false).SafeFireAndForget();
                    break;
                case UrlAction urlAction:
                    string[] segments = urlAction.Url.Segments;
                    int indexOfAction = segments.IndexOf(x => x.Contains(nameof(APIService.ShareTodo)));
                    if (indexOfAction > 0 && segments.Length >= indexOfAction + 1)
                    {
                        Guid guid = Guid.Parse(segments[indexOfAction + 1]);
                        bool IncludeFiles = await App.Current.MainPage.DisplayAlert("Descargar tarea",
                            "¿Descargar también las imágenes de esta tarea?", "Sí", "No");

                        using (Acr.UserDialogs.UserDialogs.Instance.Loading("Descargando tarea..."))
                        {
                            await APIService.DownloadSharedTodo(guid, IncludeFiles);
                        }
                    }


                    break;
            }
        }
    }
}