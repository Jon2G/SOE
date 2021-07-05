using System;
using System.Linq;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using Kit.Extensions;
using SOE.Fonts;
using SOE.Interfaces;
using SOE.ViewModels.ViewItems;
using SOE.Views.PopUps;
using SOE.Views.ViewItems;
using SOE.Views.ViewItems.ScheduleView;
using SOE.Widgets;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
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
            if (WidgetPendingAction != null)
            {
                Execute(WidgetPendingAction);
            }
        }
        #region WidgetsResponse

        private static WidgetPendingAction WidgetPendingAction;
        public static void ResponseTo(WidgetPendingAction WidgetPendingAction)
        {
            if (WidgetPendingAction is null) { return; }
            MasterPage.WidgetPendingAction = WidgetPendingAction;
            MasterPage.Instance.Execute(WidgetPendingAction);

        }
        #endregion
        private void Execute(WidgetPendingAction PendingAction)
        {
            switch (PendingAction.Action)
            {
                case TimeLineWidget.ITEM_CLICK:
                    DateTime begin = (DateTime)PendingAction.Parameters[0];
                    string group = (string)PendingAction.Parameters[1];
                    DayOfWeek dayOfClass = (DayOfWeek)PendingAction.Parameters[2];
                    MasterPage.Instance.TabView.SelectedIndex = 2;
                    Acr.UserDialogs.UserDialogs.Instance.Alert(
                        $"Ya se que presionaste:\nGrupo:{group}\nDia:{dayOfClass}\nHora:{begin:HH:mm}\npero, aún no estoy programado para abrir el menú de esta materia :)",
                        "ToDo", "Vale, apurale!");
                    break;
                case TimeLineWidget.DAY_CLICK:
                    MasterPage.Instance.TabView.SelectedIndex = 2;
                    if (MasterPage.Instance.TabView.TabItems[2].Content is ScheduleViewMain schedule)
                    {
                        DayOfWeek dayOfWeek = (DayOfWeek)PendingAction.Parameters[0];
                        var scheduleDay = schedule.Model.WeekDays.FirstOrDefault(x => x.Day.DayOfWeek == dayOfWeek);
                        schedule.OnDayTappedCommand?.Execute(scheduleDay);
                    }
                    break;

            }
            WidgetPendingAction = null;
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