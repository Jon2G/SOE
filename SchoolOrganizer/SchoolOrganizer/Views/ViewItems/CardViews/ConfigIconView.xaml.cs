using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolOrganizer.Views.ViewItems.CardViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfigIconView : ContentView
    {
        public ConfigIconView()
        {
            InitializeComponent();

            /*
             para 4 valores
            left, right,  top,  bottom
             	SetRow(view, top);
				SetRowSpan(view, bottom - top);
				SetColumn(view, left);
				SetColumnSpan(view, right - left);

            para 2:
            // left,rigth
            Add(view, left, left + 1, top, top + 1);
             */

        }
    }
}