using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolOrganizer.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskPage : PopupPage
    {

        public TaskPage()
        {
            InitializeComponent();

            Date.MinimumDate = DateTime.Now;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {

            //var tomar = new StoreCameraMediaOptions()
            //{
            //    SaveToAlbum = true,
            //    Name = "new.jpg"
            //};
            if (Xamarin.Essentials.MediaPicker.IsCaptureSupported)
            {
                var picture = await Xamarin.Essentials.MediaPicker.CapturePhotoAsync();
                if (picture != null)
                {
                    //var stream = await picture.FileName; //.GetStream();
                    //var picture = await CrossMedia.Current.TakePhotoAsync(tomar);
                    FileImageSource imagen = new FileImageSource() {File = picture.FullPath };
                    Modelo.Photos.Add(imagen);
                }
            }
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {

            var picture = await Xamarin.Essentials.MediaPicker.PickPhotoAsync(new MediaPickerOptions());

            // CrossMedia.Current.PickPhotoAsync();
            if (picture != null)
            {
                FileImageSource imagen = new FileImageSource() { File = picture.FullPath };
                Modelo.Photos.Add(imagen);
            }

        }
    }
}