using System;
using System.Collections.Generic;
using System.Text;
using SchoolOrganizer.Models.Scheduler;
using Xamarin.Forms;

namespace SchoolOrganizer.Views.ViewItems.ScheduleView
{
    public class ClassSquareDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ClassSquareTemplate { get; set; }
        public DataTemplate FreeHourTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return (item is FreeClass ? FreeHourTemplate : ClassSquareTemplate);
        }

    }
}
