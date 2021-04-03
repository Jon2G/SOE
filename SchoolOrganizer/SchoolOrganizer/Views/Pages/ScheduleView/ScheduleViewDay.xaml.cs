using System.Windows.Input;
using SchoolOrganizer.ViewModels.ViewItems;
using SchoolOrganizer.ViewModels.ViewItems.ScheduleViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolOrganizer.Views.Pages.ScheduleView
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
            get=> _MainModel;
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
            this.TomorrowCommand = new Command(Tomorrow);
            this.YesterdayCommand = new Command(Yesterday);
            InitializeComponent();
        }

        private void Yesterday()
        {
            this.DayModel =new SheduleDay(this.DayModel.Day.Yesterday());
        }

        private void Tomorrow()
        {
            this.DayModel = new SheduleDay(this.DayModel.Day.Tommorrow());
        }

        private void OnDayTapped()
        {
            Shell.Current.CurrentPage.SendBackButtonPressed();
        }

    }
}