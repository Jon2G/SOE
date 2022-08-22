using SOE.Models;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ColorPickerPage
    {
        private ViewCell lastCell;
        public ColorPickerPage(ThemeColor color)
        {
            InitializeComponent();
            this.Model.Color = color;
        }

        private void Cell_OnTapped(object sender, EventArgs e)
        {
            if (lastCell != null)
                lastCell.View.BackgroundColor = Color.Transparent;
            var viewCell = (ViewCell)sender;
            if (viewCell.View != null)
            {
                viewCell.View.BackgroundColor = Color.Accent;
                lastCell = viewCell;
            }
        }
    }
}