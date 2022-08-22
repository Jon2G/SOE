using System.Collections.Generic;

namespace SOE.Models
{
    public class ClassColors
    {
        private readonly List<ThemeColor> Colors;
        public ClassColors()
        {
            //From SELECT 'new ("'+Color+'","'+Dark_color+'"),'+CHAR(10) from APP_COLORS
            this.Colors = ThemeColor.GetAll();
        }
        internal ThemeColor Get(int v) => this.Colors.Count > v ? this.Colors[v] : this.Colors[0];
    }
}
