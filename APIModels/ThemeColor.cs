using Kit.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOEWeb.Shared
{
    public class ThemeColor : ModelBase
    {
        private string _Light;
        public string Light { get => this._Light; set { this._Light = value; Raise(() => Light); } }
        private string _Dark;
        public string Dark { get => this._Dark; set { this._Dark = value; Raise(() => Dark); } }

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
