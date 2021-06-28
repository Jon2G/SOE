using Kit.Model;

namespace SOE.Models.Scheduler
{
    public class Hour : ModelBase
    {
        public const double HourHeigth = 90;
        public readonly int HourTime;
        public string ShortName { get; }
        public  int Index { get; }
        public Hour(int Index,int HourTime)
        {
            this.Index = Index;
            this.HourTime = HourTime;
            this.ShortName = HourTime.ToString();
            if (HourTime >= 12)
            {
                ShortName += " pm";
            }
            else
            {
                ShortName += " am";
            }
        }
    }
}
