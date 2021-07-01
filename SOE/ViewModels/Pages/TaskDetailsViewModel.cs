using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FFImageLoading.Forms;
using SOE.Data;
using SOE.Data.Images;
using SOE.Enums;
using SOE.Models.TaskFirst;
using Xamarin.Forms;

namespace SOE.ViewModels.Pages
{
    public class TaskDetailsViewModel
    {
        public ToDo ToDo { get; set; }
        public ObservableCollection<PhotoArchive> Photos { get; }

        public TaskDetailsViewModel(ToDo ToDo)
        {
            this.ToDo = ToDo;
            Photos = new ObservableCollection<PhotoArchive>();
            GetPhotos();
        }

        private async void GetPhotos()
        {
            await Task.Yield();
            foreach (Archive archive in AppData.Instance.LiteConnection.Table<Archive>()
                .Where(x => x.IdKeeper == ToDo.IdKeeper))
            {
                this.Photos.Add(new PhotoArchive(archive.Path, FileType.Photo));
            }
        }
    }
}
