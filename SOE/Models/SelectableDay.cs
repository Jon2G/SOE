using SOE.Models.Scheduler;
using System;

namespace SOE.Models
{
    public class SelectableDay : SOE.Models.Scheduler.Day
    {
        private bool _IsSelected;
        public bool IsSelected
        {
            get => this._IsSelected;
            set => RaiseIfChanged(ref this._IsSelected, value);
        }
        private bool _IsSet;
        public bool IsSet { get => this._IsSet; set => RaiseIfChanged(ref this._IsSet, value); }
        public SelectableDay(Day day) : base(day.Date)
        {

        }
        public SelectableDay(DateTime Date) : base(Date)
        {
        }
    }
}
