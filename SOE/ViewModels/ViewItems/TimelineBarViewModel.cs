using System.Collections.Generic;
using System.Windows.Input;
using Kit.Model;
using SOE.Models.Scheduler;
using SOE.Views.ViewItems;
using Xamarin.Forms;

namespace SOE.ViewModels.ViewItems
{
    public class TimelineBarViewModel : ModelBase
    {
        public const double CollapsedWidth = 20;
        private const uint Speed = 150;
        private const uint Scale = 10;
        private const uint HalfScale = Scale / 2;
        public IEnumerable<ClassSquare> ClassSquares { get; set; }
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

            ClassSquares = Day.GetTimeLine();
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
                timeline.WidthRequest = CollapsedWidth * Scale;
                //timeline.ScaleXTo(Scale, Speed);
                timeline.TranslateTo(-(timeline.Width - PreviousWidth), timeline.TranslationY, Speed, Easing.SinIn);
            }
        }
    }
}
