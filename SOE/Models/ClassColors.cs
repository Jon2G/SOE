using System.Collections.Generic;

namespace SOE.Models
{
    public class ClassColors
    {
        private readonly List<ThemeColor> Colors;
        public ClassColors()
        {
            //From SELECT 'new ("'+Color+'","'+Dark_color+'"),'+CHAR(10) from APP_COLORS
            this.Colors = new List<ThemeColor>()
            {
                new("#98FFB4","#488D5A"),
                new("#6EEABE","#306D57"),
                new("#A2B4FF","#3A4265"),
                new("#A783F9","#412F6A"),
                new("#74bbfb","#3E668A"),
                new("#a1d7c9","#46675F"),
                new("#f2dea4","#BBA052"),
                new("#efc0fe","#E086FD"),
                new("#ffdac1","#7e4f09"),
                new("#fff2cc","#736409"),
                new("#e2F0cb","#657546"),
                new("#a58d7f","#7F5943"),
                new("#3E314C","#110022"),
                new("#77dd77","#56A156"),
                new("#f7cac9","#C8A4A3"),
                new("#FCB2BE","#fc6c85"),
                new("#96B2BE","#62757D"),
                new("#97BE96","#637D62"),
                new("#B6BE96","#787D62"),
                new("#C4A2A0","#9E8382"),
                new("#C8BAD4","#766D7D"),
                new("#efc0fe","#E086FD"),
                new("#e2F0cb","#657546"),
                new("#343B4F","#050d25"),
                new("#644646","#2b0202"),
                new("#d90166","#8D0143"),
                new("#355633","#062e03"),
                new("#316B74","#00424B"),
                new("#FEC1FF","#9A659A"),
                new("#FCF400","#F0E800"),
                new("#fef69e","#E9DA33"),
                new("#f2a0a1","#EE6B6D"),
                new("#b5ead7","#508676"),
                new("#c7ceea","#4c5576"),
                new("#d9d2e9","#584D71"),
                new("#FEC1FF","#9A659A"),
                new("#F9C5A2","#886146")
            };
        }
        internal ThemeColor Get(int v) => this.Colors.Count > v ? this.Colors[v] : this.Colors[0];
    }
}
