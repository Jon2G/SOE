using Kit.Model;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SOE.Models.Scheduler
{
    public class SheduleDay : ModelBase
    {
        public Day Day { get; set; }
        public ObservableCollection<ClassSquare> Class { get; set; }


        private SheduleDay(Day day, ObservableCollection<ClassSquare> classes)
        {
            this.Day = day;
            this.Class = classes;
        }

        public static Task<SheduleDay> GetDay(Day day)
        {
            return day.GetTimeLine()
                 .ContinueWith(t =>
                 {
                     var observable = new ObservableCollection<ClassSquare>(t.Result);
                     return new SheduleDay(day, observable);
                 });
        }
    }
}
