using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FFImageLoading.Forms;
using SOE.Data;
using SOE.Data.Images;
using SOE.Models.TaskFirst;
using Xamarin.Forms;

namespace SOE.ViewModels.Pages
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
