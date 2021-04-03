using System;
using System.Linq;
using Kit.Forms.Pages;
using SchoolOrganizer.Saes;
using SchoolOrganizer.Views.ViewItems;
using SchoolOrganizer.Views.ViewItems.ScheduleView;
using SchoolOrganizer.Widgets;
using Xamarin.Forms;
using SchoolGrades = SchoolOrganizer.Views.Pages.SchoolGrades;

namespace SchoolOrganizer
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell(IBrowser Browser = null)
        {
            InitializeComponent();
            //this.MasterPage.SetBrowser(Browser);
            TabBar.CurrentItem = TasksPageTab;
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
                    //MasterPage.TabView.SelectedIndex = 2;
                    Acr.UserDialogs.UserDialogs.Instance.Alert(
                        $"Ya se que presionaste:\nGrupo:{group}\nDia:{dayOfClass}\nHora:{begin:HH:mm}\npero, aún no estoy programado para abrir el menú de esta materia :)",
                        "ToDo", "Vale, apurale!");
                    break;
                case TimeLineWidget.DAY_CLICK:
                    //MasterPage.TabView.SelectedIndex = 2;
                    //if (MasterPage.TabView.TabItems[2].Content is ScheduleViewMain schedule)
                    //{
                    //    DayOfWeek dayOfWeek = (DayOfWeek)PendingAction.Parameters[0];
                    //    var scheduleDay = schedule.Model.WeekDays.FirstOrDefault(x => x.Day.DayOfWeek == dayOfWeek);
                    //    schedule.OnDayTappedCommand?.Execute(scheduleDay);
                    //}
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

        private int GetCurrentIndex(object sender)
        {
            int currentindex = 0;
            switch (sender)
            {
                case SchoolGrades:
                    currentindex = 0;
                    break;
                case TasksPage:
                    currentindex = 1;
                    break;
                case ScheduleViewMain:
                    currentindex = 2;
                    break;
            }

            return currentindex;
        }
        private void ChangeTab(int currentindex)
        {
            switch (currentindex)
            {
                case 0:
                    TabBar.CurrentItem = SchoolGradesTab;
                    break;
                case 1:
                    TabBar.CurrentItem = TasksPageTab;
                    break;
                case 2:
                    TabBar.CurrentItem = ScheduleViewMainTab;
                    break;
            }
        }
        private void SwipeGestureRecognizer_OnSwipedLeft(object sender, SwipedEventArgs e)
        {
            int currentindex = GetCurrentIndex(sender);
            currentindex++;
            if (currentindex > 2)
            {
                currentindex = 2;
            }
            ChangeTab(currentindex);
        }
        private void SwipeGestureRecognizer_OnSwipedRight(object sender, SwipedEventArgs e)
        {
            int currentindex = GetCurrentIndex(sender);
            currentindex--;
            if (currentindex < 0)
            {
                currentindex = 0;
            }
            ChangeTab(currentindex);
        }


    }
}
