using AsyncAwaitBestPractices;
using Kit.Forms;
using Kit.Model;
using SOE.Data;
using SOE.Models;
using SOE.Models.Scheduler;
using SOE.Views.Pages;
using SOE.Views.PopUps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SOE.ViewModels.Pages
{
    public class AddSubjectPageViewModel : ModelBase
    {
        public ICommand CancelCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand ChangeColorCommand { get; set; }
        public ICommand SelectDayCommand { get; set; }
        public ICommand ChangeTeacherCommand { get; set; }
        private Subject _Subject;
        public Subject Subject { get => this._Subject; set => RaiseIfChanged(ref this._Subject, value); }
        private Group _Group;
        public Group Group { get => this._Group; set => RaiseIfChanged(ref this._Group, value); }
        private SelectableDay _SelectedDay;
        public SelectableDay SelectedDay
        {
            get => this._SelectedDay;
            set
            {
                if (this._SelectedDay is { } day)
                {
                    day.IsSelected = false;
                }
                value.IsSelected = true;
                RaiseIfChanged(ref this._SelectedDay, value);
                Raise(() => SelectedDay);
            }

        }
        private Teacher _teacher;
        public Teacher Teacher
        {
            get => this._teacher;
            set => RaiseIfChanged(ref _teacher, value);

        }
        public List<SelectableDay> Days { get; set; }
        private ClassTime _classTime;
        public ClassTime ClassTime { get => this._classTime; set => RaiseIfChanged(ref this._classTime, value); }
        public Dictionary<SelectableDay, ClassTime> ClassDays { get; set; }

        private TimeSpan? _Begin;
        public TimeSpan? Begin
        {
            get => this._Begin;
            set
            {
                this._Begin = value;
                if (ClassTime is not null && value is TimeSpan time)
                {
                    SelectedDay.IsSet = true;
                    ClassTime.Begin = time;
                }
                Raise(() => this.Begin);
            }
        }
        private TimeSpan? _End;
        public TimeSpan? End
        {
            get => this._End;
            set
            {
                this._End = value;
                if (ClassTime is not null && value is TimeSpan time)
                {
                    SelectedDay.IsSet = true;
                    ClassTime.End = time;
                }
                Raise(() => this._End);
            }
        }

        public AddSubjectPageViewModel()
        {
            Subject = new Subject();
            Teacher = new Teacher();
            Group = new Group();
            CancelCommand = new AsyncFeedbackCommand(Close);
            SaveCommand = new AsyncFeedbackCommand(Save);
            ChangeColorCommand = new AsyncFeedbackCommand(ChangeColor);
            SelectDayCommand = new Command<SelectableDay>(SelectDay);
            ChangeTeacherCommand = new Command(ChangeTeacher);
            ClassDays = new Dictionary<SelectableDay, ClassTime>();
            Days = new List<SelectableDay>(new[]
            {
                new SelectableDay(Day.GetNearest(DayOfWeek.Monday)),
                new SelectableDay(Day.GetNearest(DayOfWeek.Tuesday)),
                new SelectableDay(Day.GetNearest(DayOfWeek.Wednesday)),
                new SelectableDay(Day.GetNearest(DayOfWeek.Thursday)),
                new SelectableDay(Day.GetNearest(DayOfWeek.Friday)),
            });
            SelectDay(Days.First());
            this.Init().SafeFireAndForget();
        }

        private void ChangeTeacher()
        {
            new AddTeacherPopUp()
                .ShowDialog()
                .ContinueWith(t =>
                {
                    AddTeacherPopUp popUp = (AddTeacherPopUp)t.Result;
                    this.Teacher = new Teacher() { Name = popUp.ViewModel.TeacherName };
                    return popUp;
                }).SafeFireAndForget();
        }
        private void SelectDay(SelectableDay day)
        {
            SelectedDay = day;
            if (ClassDays.TryGetValue(day, out ClassTime _ClassTime))
            {
                ClassTime = _ClassTime;
                return;
            }
            ClassTime = new ClassTime() { Day = day.DayOfWeek, Subject = Subject, };
            ClassDays.Add(day, ClassTime);
            GetSmartClassTime(day).SafeFireAndForget();
        }

        private async Task GetSmartClassTime(Day day)
        {
            ClassTime? classTime = await ClassTime.EarlierClassOfWeek();
            if (classTime is null)
            {
                return;
            }
            TimeSpan? suggestedBegin = classTime.Begin;
            TimeSpan? suggestedEnd = classTime.End;
            if (classTime.Day == day.DayOfWeek)
            {
                suggestedEnd = null;
                suggestedBegin = await ClassTime.GetFirstFreeTimeOf(day);
                if (suggestedBegin is null)
                {
                    classTime = await ClassTime.GetLastClassOf(day);
                    suggestedBegin = classTime.End;
                }
            }

            if (suggestedEnd is null && suggestedBegin is not null)
            {
                TimeSpan? time = await ClassTime.GetAvgClassDuration();
                if (time is { } t)
                    suggestedEnd = suggestedBegin.Value.Add(t);
            }
            Begin = suggestedBegin;
            End = suggestedEnd;
        }

        private async Task Init()
        {
            Subject.
            ThemeColor = await ThemeColor.GetUnusedColor();
        }

        public async Task ChangeColor()
        {
            ColorPickerPage? page = new ColorPickerPage(this.Subject.ThemeColor);
            await Shell.Current.Navigation.PushAsync(page);
            await page.WaitUntilClose();
            this.Subject.ThemeColor = page.Model.Color;
        }
        private async Task Save()
        {
            await this.Teacher.Save();
            await this.Group.Save();
            this.Subject.GroupId = this.Group.GetDocumentId();
            this.Subject.TeacherId = this.Teacher.GetDocumentId();
            await this.Subject.Save();
            foreach (KeyValuePair<SelectableDay, ClassTime> classDay in this.ClassDays)
            {
                if (classDay.Key.IsSelected)
                {
                    classDay.Value.GroupId = this.Group.GetDocumentId();
                    classDay.Value.SubjectId = this.Subject.GetDocumentId();
                    await classDay.Value.Save();
                }
            }
            if (!AppData.Instance.User.HasSubjects)
            {
                AppData.Instance.User.HasSubjects = true;
                await AppData.Instance.User.Save();
            }
            this.Close();
        }
        private void Close()
        {
            Shell.Current.Navigation.PopAsync().SafeFireAndForget();
        }
    }
}
