﻿using System;
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
        public static void ResponseTo(PendingAction PendingAction) => App.Current.Dispatcher.BeginInvokeOnMainThread(action: () => Execute(PendingAction));
        private static async void Execute(PendingAction pendingAction)
        {
            switch (pendingAction)
            {
                case TimeLineWidgetDayAction dayAction:
                    MasterPage.Instance.Model.SelectedIndex = 2;
                    ViewItems.ScheduleView.ScheduleViewMain.Instance
                        .Model.WeekDays.FirstOrDefault(x => x.Day.DayOfWeek == dayAction.Day);
                    ViewItems.ScheduleView.ScheduleViewMain.Instance.OnDayTappedCommand?.Execute(dayAction.Day);
                    break;
                case TimeLineWidgetSubjectAction subjectAction:
                    MasterPage.Instance.Model.SelectedIndex = 2;
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
    }
}