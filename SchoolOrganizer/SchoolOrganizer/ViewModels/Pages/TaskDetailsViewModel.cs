using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using FFImageLoading.Forms;
using SchoolOrganizer.Data;
using SchoolOrganizer.Data.Images;
using SchoolOrganizer.Enums;
using SchoolOrganizer.Models.TaskFirst;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SchoolOrganizer.ViewModels.Pages
{
    public class TaskDetailsViewModel
    {
        public ToDo ToDo { get; set; }
        public ObservableCollection<Archive<CachedImage>> Photos { get; }

        public TaskDetailsViewModel(ToDo ToDo)
        {
            this.ToDo = ToDo;
            Photos = new ObservableCollection<Archive<CachedImage>>();
            GetPhotos();
        }

        private async void GetPhotos()
        {
            await Task.Yield();
            foreach (Archive archive in AppData.Instance.LiteConnection.Table<Archive>()
                .Where(x => x.IdKeeper == ToDo.IdKeeper))
            {
                this.Photos.Add(new Archive<CachedImage>(archive,new CachedImage()
                {
                    Source = FileImageSource.FromFile(archive.Path),
                    DownsampleToViewSize = true
                }));
            }
        }
    }
}
