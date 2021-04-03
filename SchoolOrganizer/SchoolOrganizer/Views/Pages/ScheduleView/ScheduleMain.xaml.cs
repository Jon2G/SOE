using System.Threading.Tasks;
using System.Windows.Input;
using SchoolOrganizer.ViewModels.ViewItems.ScheduleViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolOrganizer.Views.Pages.ScheduleView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScheduleViewMain 
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

        private void OnDayTapped(SheduleDay day)
        {
            if (day is null) { return; }
            DayView.DayModel = day;
            DayView.MainModel = this.Model;
            Fade(true);

            if (Shell.Current?.CurrentPage is MasterPage master)
            {
                master.TabView.IsSwipeEnabled = false;
            }
        }

        internal bool OnBackButtonPressed()
        {
            if (IsDayViewVisible)
            {
                Fade(false);
                if (Shell.Current.CurrentPage is MasterPage master)
                {
                    master.TabView.IsSwipeEnabled = true;
                }
                return true;
            }
            return false;
        }

        private async void Fade(bool IsDayViewVisible)
        {
            if (IsDayViewVisible)
            {
                this.IsDayViewVisible = IsDayViewVisible;
                await Task.Delay(100);
            }
            await DayView.FadeTo(IsDayViewVisible ? 1 : 0, 500u, Easing.Linear);
            this.IsDayViewVisible = IsDayViewVisible;
        }
    }
}