using Kit.Model;
using LiteDB;
using SOE.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOE.Models
{
    public class ThemeColor : ModelBase, IComparable, IComparable<ThemeColor>
    {
        public ObjectId Id { get; set; }
        private string _Light;

        public string Light { get => this._Light; set { this._Light = value; this.Raise(() => this.Light); } }
        private string _Dark;

        public string Dark { get => this._Dark; set { this._Dark = value; this.Raise(() => this.Dark); } }

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

        public static ILiteCollection<ThemeColor> Collection =>
            AppData.Instance.LiteDatabase.GetCollection<ThemeColor>();
        public void Save()
        {
            Collection.Upsert(this);
        }
        public static List<ThemeColor> GetAll()
        {
            List<ThemeColor> colors = Collection
                .FindAll()?.ToList() ?? new List<ThemeColor>();
            colors.AddRange(new ThemeColor[]
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
            });
            return colors;
        }

        internal static async Task<ThemeColor> GetUnusedColor()
        {
            var subjects = await Subject.GetAll();
            var usedColors = subjects.Select(x => x.ThemeColor);
            var allcolors = GetAll();
            return allcolors.FirstOrDefault(x => usedColors.All(y => y.CompareTo(x) != 0)) ?? allcolors.First();
        }

        public int CompareTo(object obj)
        {
            if (obj is ThemeColor color)
            {
                return color.CompareTo(this);
            }

            return -1;
        }

        public int CompareTo(ThemeColor other)
        {
            return other?.Light?.CompareTo(this.Light) ?? -1;
        }
    }
}
