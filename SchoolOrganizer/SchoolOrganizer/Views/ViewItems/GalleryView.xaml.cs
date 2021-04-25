using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kit.Forms.Controls;
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

        public async void Show(IEnumerable<FileImageSource> photos, FileImageSource seleccionada)
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

        private void ZoomGestureContainer_OnOnSwiped(object sender, EventArgs e)
        {
            if (sender is ZoomGestureContainer content)
            {
                if (content.Content.TranslationY >= 70 && content.CurrentScale == 1)
                {
                    content.Content.TranslationY = 0;
                    this.Hide();
                }

                if (content.Content.TranslationX >= 70 && content.CurrentScale == 1)
                {
                    content.Content.TranslationX = 0;
                }
            }
        }
    }
}