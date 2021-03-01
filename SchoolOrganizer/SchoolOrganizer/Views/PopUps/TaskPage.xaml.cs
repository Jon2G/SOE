using Plugin.Media;
using Plugin.Media.Abstractions;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var tomar = new StoreCameraMediaOptions()
            {
                SaveToAlbum = true,
                Name = "new.jpg"
            };
            var picture = await CrossMedia.Current.TakePhotoAsync(tomar);
            MyImg.Source = ImageSource.FromStream(() =>
            {
                var stream = picture.GetStream();
                picture.Dispose();
                return stream;
            });
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            if (CrossMedia.Current.IsTakePhotoSupported)
            {
                var picture = await CrossMedia.Current.PickPhotoAsync();
                if (picture != null)
                {
                    MyImg.Source = ImageSource.FromStream(() =>
                    {
                        var stream = picture.GetStream();
                        picture.Dispose();
                        return stream;
                    });
                }
            }
        }
    }
}