using System;
using System.Threading.Tasks;
using System.Windows.Input;
using SOEWeb.Shared;
using SOE.Data;
using SOE.Models.Scheduler;
using SOE.Models.TaskFirst;
using SOE.Views.Pages;
using SOE.Views.PopUps;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Kit;
using SOE.Fonts;

namespace SOE.Views.ViewItems.ScheduleView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScheduleViewMain
    {
        public override string Title => "Horario";
        public override string Icon => FontelloIcons.Calendar;
        public static ScheduleViewMain Instance { get; private set; }
        private bool IsDayViewVisible
        {
            get => DayView.IsVisible;
            set
            {
                DayView.IsTabStop = value;
                DayView.IsEnabled = value;
                DayView.IsVisible = value;
            }
        }

        private ICommand _OnDayTappedCommand;
        public ICommand OnDayTappedCommand => _OnDayTappedCommand ??= new Command<SheduleDay>(OnDayTapped);


        public ICommand _OpenMenuCommand;
        public ICommand OpenMenuCommand => _OpenMenuCommand ??= new Command<ClassSquare>(OpenMenu);
        public ScheduleViewMain()
        {
            Instance = this;
            InitializeComponent();
            this.ToolbarItem.Command = this.Model.ExportToPdfCommand;
            if (!AppData.Instance.User.HasSubjects)
            {
                this.Content = new NoInscriptionView();
                return;
            }
            this.IsDayViewVisible = false;

        }
        private async void OpenMenu(ClassSquare classSquare)
        {
            var pr = new MenuHorarioPopUp(classSquare);
            await pr.ShowDialog();
                switch (pr.Model.Action)
                {
                    case "Nueva tarea":
                        Newtask(classSquare);
                        break;
                    case "Recordatorio":
                        Remember();
                        break;
                 case "Info Materia":
                     InfoSub(classSquare.Subject);
                    break;   
            }
        }

        private void InfoSub(Subject subject) => 
            Shell.Current.Navigation.PushAsync(new SubjectPage(subject));

        private void Remember()
        {
            
        }

        private void Newtask(ClassSquare classSquare)
        {
            var Tarea = new ToDo();
            Tarea.Subject = classSquare.Subject;
            Tarea.Time = classSquare.Begin;
            Tarea.Date = classSquare.Day.GetNearest();
            App.Current.MainPage.Navigation.PushAsync(new NewTaskPage(Tarea));
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