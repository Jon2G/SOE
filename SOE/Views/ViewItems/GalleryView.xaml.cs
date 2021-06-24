using System;
using System.Collections.Generic;
using FFImageLoading.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.ViewItems
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GalleryView : ContentView
    {
        public GalleryView()
        {
            InitializeComponent();
        }

        public async void Show(IEnumerable<CachedImage> photos, CachedImage seleccionada)
        {
            this.IsVisible = true;
            await this.FadeTo(1, 500, Easing.Linear);
            this.Model.SendImages(photos, seleccionada);
        }

        public async void Hide()
        {
            await this.FadeTo(0, 500, Easing.Linear);
            this.IsVisible = false;
        }

        private void GoBack_Tapped(object sender, EventArgs e)
        {
           this.Hide();
        }
    }
}