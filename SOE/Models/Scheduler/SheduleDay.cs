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

        public static async Task<SheduleDay> GetDay(Day day)
        {
            var observable = new ObservableCollection<ClassSquare>(await day.GetTimeLine());
            return new SheduleDay(day, observable);
        }
    }
}
