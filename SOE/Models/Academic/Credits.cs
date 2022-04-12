using Kit.Sql.Attributes;
using Plugin.CloudFirestore.Attributes;

namespace SOE.Models.Academic
{
    [Preserve(AllMembers = true)]
    public class Credits
    {
        public float CurrentCredits { get; set; }

        public float TotalCredits { get; set; }

        public float Percentage { get; set; }
        [Ignored]
        public float Progress => Percentage / 100;

        public Credits() { }
    }
}
