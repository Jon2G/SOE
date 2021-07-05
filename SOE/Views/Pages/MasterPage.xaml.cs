using System;
using System.Linq;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using Kit;
using Kit.Extensions;
using SOE.API;
using SOE.Data;
using SOE.Fonts;
using SOE.Interfaces;
using SOE.Models.TaskFirst;
using SOE.Services.ActionResponse;
using SOE.ViewModels.ViewItems;
using SOE.Views.PopUps;
using SOE.Views.ViewItems;
using SOE.Views.ViewItems.ScheduleView;
using SOE.Widgets;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using Command = Xamarin.Forms.Command;

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


        //private async void Button_Clicked(object sender, EventArgs e)
        //{

        //}
        protected override bool OnBackButtonPressed()
        {
            if (TabView.SelectedIndex < 0)
            {
                TabView.SelectedIndex = 0;
            }
            TabViewItem selectedView = this.TabView.TabItems[TabView.SelectedIndex];
            switch (selectedView.Content)
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
                Dispatcher.BeginInvokeOnMainThread(ShowTodoIcon);
            }
            else if (TabView.SelectedIndex <= 0)
            {
                Dispatcher.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(100);
                    TabView.SelectedIndex = 1;
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
                    MasterPage.Instance.TabView.SelectedIndex = 2;
                    ViewItems.ScheduleView.ScheduleViewMain.Instance
                        .Model.WeekDays.FirstOrDefault(x => x.Day.DayOfWeek == dayAction.Day);
                    ViewItems.ScheduleView.ScheduleViewMain.Instance.OnDayTappedCommand?.Execute(dayAction.Day);
                    break;
                case TimeLineWidgetSubjectAction subjectAction:
                    MasterPage.Instance.TabView.SelectedIndex = 2;
                    Acr.UserDialogs.UserDialogs.Instance.Alert(
                        $"Ya se que presionaste:\nGrupo:{subjectAction.Group}\nDia:{subjectAction.Day.Dia()}\nHora:{subjectAction.Date:HH:mm}\npero, aún no estoy programado para abrir el menú de esta materia :)",
                        "ToDo", "Vale, apurale!");
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
                    if (indexOfAction > 0 && segments.Length >= indexOfAction+1)
                    {
                        Guid guid = Guid.Parse(segments[indexOfAction+1]);
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

        public void ShowHorarioIcon()
        {
            this.Title = "Horario";
            this.ToolbarItems.Clear();
            this.ToolbarItems.Add(new ToolbarItem
            {
                Command = ScheduleViewMain.Model.ExportToPdfCommand,
                CommandParameter = ScheduleViewMain,
                IconImageSource = new FontImageSource()
                {
                    FontFamily = FontelloIcons.Font,
                    Glyph = FontelloIcons.PDF
                }
            });
        }
        public void ShowTodoIcon()
        {
            this.Title = "Tareas";
            this.ToolbarItems.Clear();
            this.ToolbarItems.Add(new ToolbarItem
            {
                Command = TaskFirstViewModel.Instance?.AddTaskCommand,
                CommandParameter = this,
                IconImageSource = new FontImageSource()
                {
                    FontFamily = FontelloIcons.Font,
                    Glyph = FontelloIcons.CirclePlus
                }
            });
        }

        private void TabView_OnSelectionChanged(object sender, TabSelectionChangedEventArgs e)
        {
            switch (TabView.SelectedIndex)
            {
                case 0:
                    this.Title = "Calificaciones";
                    this.ToolbarItems.Clear();
                    this.ToolbarItems.Add(new ToolbarItem
                    {
                        Command = this.SchoolGrades.Model?.RefreshCommand,
                        CommandParameter = this,
                        IconImageSource = new FontImageSource()
                        {
                            FontFamily = FontelloIcons.Font,
                            Glyph = FontelloIcons.Refresh
                        }
                    });
                    break;
                case 1:
                    ShowTodoIcon();
                    break;
                case 2:
                    ShowHorarioIcon();
                    break;
                default:
                    this.ToolbarItems.Clear();
                    break;
            }
        }

    }
}