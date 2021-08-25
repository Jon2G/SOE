using SOE.Fonts;
using Xamarin.Forms;
using ContentView = Forms9Patch.ContentView;

namespace SOE.Models
{
    public class IconView : ContentView
    {
        public virtual string Title => string.Empty;
        public virtual void OnAppearing(){}
        public IconView() : base()
        {

        }
    }
}
