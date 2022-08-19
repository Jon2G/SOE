using AsyncAwaitBestPractices;
using Kit;
using SOE.Data;
using SOE.Enums;
using SOE.Models;
using SOE.Models.Scheduler;
using SOE.Models.TodoModels;
using SOE.Views.Pages;
using SOE.Views.PopUps;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Device = Xamarin.Forms.Device;

namespace SOE.Views.ViewItems.ScheduleView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScheduleViewMain
    {
        public override string Title => "Horario";
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
            Tools.Container.Register(this);
            InitializeComponent();
            this.Init();
        }

        public void Init()
        {
            if (!AppData.Instance.User.HasSubjects)
            {
                this.Content =
                    AppData.Instance.User.Mode == UserMode.SAES ?
                        new NoInscriptionView() :
                        new NoSubjectsView();
                return;
            }
            this.IsDayViewVisible = false;
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
        }

        private void OpenMenu(ClassSquare classSquare)
        {
            MenuHorarioPopUp? pr = new MenuHorarioPopUp(classSquare);
            pr.ShowDialog().ContinueWith(t =>
            {
                switch (pr.Model.Action)
                {
                    case "Nueva tarea":
                        Newtask(classSquare);
                        break;
                    case "Recordatorio":
                        Reminder(classSquare);
                        break;
                    case "Blog":
                        Blog(classSquare.Subject);
                        break;
                    case "Links":
                        ShowLinks(classSquare);
                        break;
                }
            }).SafeFireAndForget();
        }

        private void ShowLinks(ClassSquare classSquare)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Shell.Current.Navigation.PushAsync(new LinksPage(classSquare));
            });
        }

        private void Blog(Subject subject)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Shell.Current.Navigation.PushAsync(new BlogPage(subject));
            });
        }


        private void Reminder(ClassSquare classSquare)
        {
            Reminder? reminder = new Reminder();
            reminder.Subject = classSquare.Subject;
            reminder.Time = classSquare.Begin;
            reminder.Date = classSquare.Day.GetNearest();
            ReminderPage? popup = new ReminderPage(reminder);
            popup.Show().SafeFireAndForget();
        }

        private void Newtask(ClassSquare classSquare)
        {
            ToDo? Tarea = new ToDo();
            Tarea.Subject = classSquare.Subject;
            Tarea.Time = classSquare.Begin;
            Tarea.Date = classSquare.Day.GetNextOrToday();
            Device.BeginInvokeOnMainThread(() =>
            {
                App.Current.MainPage.Navigation.PushAsync(new NewTaskPage(Tarea));
            });
        }





        private void OnDayTapped(SheduleDay day)
        {
            if (day is null) { return; }
            DayView.DayModel = day;
            DayView.MainModel = this.Model;
            Fade(true).SafeFireAndForget();
        }

        internal bool OnBackButtonPressed()
        {
            if (IsDayViewVisible)
            {
                Fade(false).SafeFireAndForget();
                return true;
            }
            return false;
        }

        private async Task Fade(bool IsDayViewVisible)
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