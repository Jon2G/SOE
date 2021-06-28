﻿using System;
using System.Collections.Generic;
using System.Text;
using Kit.Forms.Fonts;
using FontelloIcons = SchoolOrganizer.Fonts.FontelloIcons;

namespace SchoolOrganizer.Models.Scheduler
{
    public class FreeClass:ClassSquare
    {
        public static int Count=0;

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

        public FreeClass(TimeSpan Begin, TimeSpan End, DayOfWeek Day) :
            base(Subject.FreeHour(), Begin, End, Day)
        {
          
        }
    }
}