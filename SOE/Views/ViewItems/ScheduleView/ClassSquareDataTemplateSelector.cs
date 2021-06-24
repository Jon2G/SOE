using SOE.Models.Scheduler;
using Xamarin.Forms;

namespace SOE.Views.ViewItems.ScheduleView
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
