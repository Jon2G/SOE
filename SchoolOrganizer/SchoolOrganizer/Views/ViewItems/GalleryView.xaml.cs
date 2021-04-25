using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolOrganizer.Views.ViewItems
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GalleryView : ContentView
    {
        public GalleryView()
        {
            InitializeComponent();
        }

        public async void Show()
        {
            this.IsVisible = true;
            await this.FadeTo(1, 500, Easing.Linear);
        }

        public async void Hide()
        {
            await this.FadeTo(0, 500, Easing.Linear);
            this.IsVisible = false;
        }
    }
}