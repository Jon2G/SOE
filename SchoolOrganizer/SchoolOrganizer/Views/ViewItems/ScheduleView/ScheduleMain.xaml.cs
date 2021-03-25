using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SchoolOrganizer.Models.Scheduler;
using SchoolOrganizer.ViewModels.ViewItems.ScheduleViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolOrganizer.Views.ViewItems.ScheduleView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScheduleViewMain : ContentView
    {
        private bool IsDayViewVisible
        {
            get => DayView.IsVisible;
            set
            {
                DayView.IsVisible = value;
            }
        }

        public ICommand OnDayTappedCommand { get; }
        public ScheduleViewMain()
        {
            this.OnDayTappedCommand = new Command<SheduleDay>(OnDayTapped);
            InitializeComponent();
            this.IsDayViewVisible = false;
        }

        private async void OnDayTapped(SheduleDay day)
        {
            if (day is null) { return; }
            DayView.DayModel = day;
            DayView.MainModel = this.Model;
            await DayView.FadeTo(1, 250u, Easing.Linear);
            IsDayViewVisible = true;
        }

        internal bool OnBackButtonPressed()
        {
            if (IsDayViewVisible)
            {
                DayView.FadeTo(0, 250u, Easing.Linear);
                IsDayViewVisible = false;
                return true;
            }
            return false;
        }
    }
}