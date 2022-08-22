using AsyncAwaitBestPractices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.ViewItems
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GalleryView
    {
        public GalleryView()
        {
            InitializeComponent();
        }

        public async Task Show(IEnumerable<ImageSource> photos, ImageSource seleccionada)
        {
            Shell.SetNavBarIsVisible(Shell.Current.CurrentPage, false);
            this.IsVisible = true;
            await this.FadeTo(1, 500, Easing.Linear);
            this.Model.SendImages(photos, seleccionada);
        }

        public async Task Hide()
        {
            await this.FadeTo(0, 500, Easing.Linear);
            this.IsVisible = false;
            Shell.SetNavBarIsVisible(Shell.Current.CurrentPage, true);
        }

        private void GoBack_Tapped(object sender, EventArgs e)
        {
            this.Hide().SafeFireAndForget();
        }
    }
}