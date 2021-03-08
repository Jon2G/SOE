using System;
using Rg.Plugins.Popup.Pages;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolOrganizer.Views.PopUps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskPage : PopupPage
    {
        private int _position;
        public int Position { get { return _position; } set { _position = value; OnPropertyChanged(); } }
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
            //};
            if (Xamarin.Essentials.MediaPicker.IsCaptureSupported)
            {
                var picture = await Xamarin.Essentials.MediaPicker.CapturePhotoAsync();
                if (picture != null)
                {
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