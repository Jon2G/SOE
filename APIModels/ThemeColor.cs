using System;
using System.Collections.Generic;
using System.Text;

namespace SOEWeb.Shared
{
    public class ThemeColor
    {
        public string Light { get; set; }
        public string Dark { get; set; }

        public ThemeColor(string color)
        {
            this.Dark = color;
            this.Light = color;
        }
        public ThemeColor(string Light, string Dark)
        {
            this.Light = Light;
            this.Dark = Dark;
        }
        public ThemeColor()
        {

        }
    }
}
