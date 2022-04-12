using System;
using FontelloIcons = SOE.Fonts.FontelloIcons;

namespace SOE.Models.Scheduler
{
    public class FreeClass : ClassSquare
    {
        private static int Count = 0;

        public string Icon
        {
            get
            {
                Count++;
                if (Count >= 4)
                {
                    Count = 1;
                }
                switch (Count)
                {
                    case 1:
                        return FontelloIcons.HappyTea;
                    case 2:
                        return FontelloIcons.HappyBeer;
                    case 3:
                        return FontelloIcons.HappyGlasses;
                    case 4:
                        return FontelloIcons.HappyLike;
                    default:
                        return FontelloIcons.HappyTea;

                }
            }
        }

        public FreeClass(TimeSpan begin, TimeSpan end, DayOfWeek day) :
            base(Subject.FreeTime, Group.None, begin, end, day)
        {

        }
    }
}
