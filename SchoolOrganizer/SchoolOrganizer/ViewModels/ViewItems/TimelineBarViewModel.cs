using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SchoolOrganizer.Models.Scheduler;
using SchoolOrganizer.Views.ViewItems;
using Xamarin.Forms;

namespace SchoolOrganizer.ViewModels.ViewItems
{
    public class TimelineBarViewModel : ViewModels.Pages.BaseViewModel
    {
        public const double CollapsedWidth = 20;
        private const uint Speed = 150;
        private const uint Scale = 10;
        private const uint HalfScale = Scale / 2;
        public IEnumerable<Subject> Subjects { get; set; }
        private readonly Day Day;

        private bool _IsExpanded;
        public bool IsExpanded
        {
            get => _IsExpanded;
            set
            {
                _IsExpanded = value;
                OnPropertyChanged(nameof(IsExpanded));
            }
        }
        public ICommand ExpandCommand { get; set; }
        public ICommand CollapseCommand { get; set; }
        public ICommand ToggleCommand { get; set; }
        public TimelineBarViewModel()
        {
            Day = Day.Today();
            Subjects = Day.GetTimeLine();
            this.IsExpanded = false;
            this.ExpandCommand = new Command(Expand);
            this.CollapseCommand = new Command(Collapse);
            this.ToggleCommand = new Command(ToggleMenu);
        }

        private void ToggleMenu(object obj)
        {
            IsExpanded = !IsExpanded;
            Toggle(obj);
        }

        private void Collapse(object obj)
        {
            if (IsExpanded)
            {
                ToggleMenu(obj);
            }
        }

        private void Expand(object obj)
        {
            if (!IsExpanded)
            {
                ToggleMenu(obj);
            }
        }

        private void Toggle(object obj)
        {
            if (!(obj is TimelineBar timeline)) return;
            if (!IsExpanded)
            {
               // timeline.ScaleXTo(1, Speed);
               timeline.WidthRequest = CollapsedWidth;
                timeline.TranslateTo(0, 0, Speed, Easing.SinInOut);
            }
            else
            {
                double PreviousWidth = timeline.Width;
                //timeline.ScaleXTo(Scale, Speed);
                timeline.WidthRequest = CollapsedWidth* Scale;
                //timeline.ScaleXTo(Scale, Speed);
                timeline.TranslateTo(-(timeline.Width-PreviousWidth), timeline.TranslationY, Speed,Easing.SinIn);
            }
        }
    }
}
