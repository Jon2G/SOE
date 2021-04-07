using System;
using FFImageLoading.Cache;
using FFImageLoading.Forms;
using FFImageLoading.Work;
using Rg.Plugins.Popup.Pages;
using SchoolOrganizer.Models.TaskFirst;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolOrganizer.Views.PopUps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskPage : ContentPage
    {
        private int _position;
        public int Position { get { return _position; } set { _position = value; OnPropertyChanged(); } }
        public TaskPage()
        {
            InitializeComponent();

        }

        private async void Button_Clicked(object sender, EventArgs e)
        {

            if (Xamarin.Essentials.MediaPicker.IsCaptureSupported)
            {
                var picture = await Xamarin.Essentials.MediaPicker.CapturePhotoAsync();

                if (picture != null)
                {
                    FileImageSource imagen = new FileImageSource() {File = picture.FullPath };


                    var cachedImage = new CachedImage()
                    {
                        CacheType = CacheType.Disk,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        DownsampleToViewSize = true,
                        Source = imagen
                    };


                    Modelo.Photos.Add(imagen);
                }
            }
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {

            var picture = await Xamarin.Essentials.MediaPicker.PickPhotoAsync(new MediaPickerOptions());

            if (picture != null)
            {
                FileImageSource imagen = new FileImageSource() { File = picture.FullPath };

                var cachedImage = new CachedImage()
                {
                    CacheType = CacheType.Disk,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    DownsampleToViewSize = true,
                    Source = imagen
                };

                Modelo.Photos.Add(imagen);
            }

        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            var Task = this.Modelo.Tarea;
            this.Modelo.Tareas.Add(Task);
            this.Modelo.Tarea = new ToDo();
            
        }
    }
}