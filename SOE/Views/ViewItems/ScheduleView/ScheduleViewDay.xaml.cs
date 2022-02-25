using AsyncAwaitBestPractices.MVVM;
using SOE.Models.Scheduler;
using SOE.ViewModels.ViewItems;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.ViewItems.ScheduleView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScheduleViewDay : ContentView
    {
        public SheduleDay DayModel
        {
            get => this.BindingContext as SheduleDay;
            set
            {
                this.BindingContext = value;
                OnPropertyChanged();
            }
        }

        private ScheduleMainViewModel _MainModel;
        public ScheduleMainViewModel MainModel
        {
            get => _MainModel;
            set
            {
                _MainModel = value;
                OnPropertyChanged();
            }
        }
        public ICommand OnDayTappedCommand { get; }
        public ICommand TomorrowCommand
        {
            get;
        }
        public ICommand YesterdayCommand
        {
            get;
        }



        public ScheduleViewDay()
        {
            this.OnDayTappedCommand = new Command(OnDayTapped);
            this.TomorrowCommand = new AsyncCommand(Tomorrow);
            this.YesterdayCommand = new AsyncCommand(Yesterday);
            InitializeComponent();
        }

        private async Task Yesterday()
        {
            this.DayModel = await SheduleDay.GetDay(this.DayModel.Day.Yesterday());
        }

        private async Task Tomorrow()
        {
            this.DayModel = await SheduleDay.GetDay(this.DayModel.Day.Tommorrow());
        }

        private void OnDayTapped()
        {
            Shell.Current.CurrentPage.SendBackButtonPressed();
        }

    }
}