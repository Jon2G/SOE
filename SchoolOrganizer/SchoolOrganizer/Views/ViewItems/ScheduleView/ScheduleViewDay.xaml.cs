using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SchoolOrganizer.ViewModels.ViewItems;
using SchoolOrganizer.ViewModels.ViewItems.ScheduleViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolOrganizer.Views.ViewItems.ScheduleView
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
        public ScheduleViewDay()
        {
            this.OnDayTappedCommand = new Command(OnDayTapped);
            InitializeComponent();
        }
        private void OnDayTapped()
        {
            Shell.Current.CurrentPage.SendBackButtonPressed();
        }

    }
}