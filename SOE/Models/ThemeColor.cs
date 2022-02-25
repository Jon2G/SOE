using Google.Cloud.Firestore;
using Kit.Model;

namespace SOE.Models
{
    [FirestoreData]
    public class ThemeColor : ModelBase
    {
        private string _Light;
        [FirestoreProperty]
        public string Light { get => this._Light; set { this._Light = value; this.Raise(() => this.Light); } }
        private string _Dark;
        [FirestoreProperty]
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
    }
}
