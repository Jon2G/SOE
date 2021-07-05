using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FFImageLoading.Forms;
using P42.Utils;
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
            this.Photos.AddRange(Models.TaskFirst.ToDo.GetPhotos(this.ToDo));
        }
    }
}
