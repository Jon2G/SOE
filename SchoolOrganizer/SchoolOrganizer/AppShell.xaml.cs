using System;
using System.Linq;
using Kit.Forms.Pages;
using SchoolOrganizer.Saes;
using SchoolOrganizer.Views.Pages;
using SchoolOrganizer.Views.ViewItems.ScheduleView;
using SchoolOrganizer.Widgets;
using Xamarin.Forms;

namespace SchoolOrganizer
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell(IBrowser Browser = null)
        {
            InitializeComponent();
            this.MasterPage.SetBrowser(Browser);
        }

        #region WidgetsResponse

        private static WidgetPendingAction WidgetPendingAction;
        public static void ResponseTo(WidgetPendingAction WidgetPendingAction)
        {
            if (WidgetPendingAction is null) { return; }
            AppShell.WidgetPendingAction = WidgetPendingAction;
            if (Shell.Current != null)
            {
                (Shell.Current as AppShell)?.Execute(WidgetPendingAction);
            }
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
                    MasterPage.TabView.SelectedIndex = 2;
                    Acr.UserDialogs.UserDialogs.Instance.Alert(
                        $"Ya se que presionaste:\nGrupo:{group}\nDia:{dayOfClass}\nHora:{begin:HH:mm}\npero, aún no estoy programado para abrir el menú de esta materia :)",
                        "ToDo", "Vale, apurale!");
                    break;
                case TimeLineWidget.DAY_CLICK:
                    MasterPage.TabView.SelectedIndex = 2;
                    if (MasterPage.TabView.TabItems[2].Content is ScheduleViewMain schedule)
                    {
                        DayOfWeek dayOfWeek = (DayOfWeek)PendingAction.Parameters[0];
                        var scheduleDay = schedule.Model.WeekDays.FirstOrDefault(x => x.Day.DayOfWeek == dayOfWeek);
                        schedule.OnDayTappedCommand?.Execute(scheduleDay);
                    }
                    break;
            }
            AppShell.WidgetPendingAction = null;
        }
        private void MasterPage_OnAppearing(object sender, EventArgs e)
        {
            if (WidgetPendingAction != null)
            {
                Execute(WidgetPendingAction);
            }
        }
    }
}
