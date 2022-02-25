using AsyncAwaitBestPractices;
using P42.Utils;
using SOE.Data.Archives;
using SOE.Models.TodoModels;
using System.Collections.ObjectModel;
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

        private void GetPhotos()
        {
            this.ToDo.GetPhotos().ContinueWith(t =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.Photos.AddRange(t.Result);
                });
            }).SafeFireAndForget();
        }
    }
}
