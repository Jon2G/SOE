using System.Linq;
using System.Windows.Input;
using FFImageLoading.Forms;
using SOE.Data.Images;
using SOE.Models.TaskFirst;
using SOE.ViewModels.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Skip)]
    public partial class TaskDetails : ContentPage
    {
        public TaskDetailsViewModel Model { get; set; }
        public ICommand GalleryViewCommand { get; }

        public TaskDetails(ToDo todo)
        {
            this.Model = new TaskDetailsViewModel(todo);
            BindingContext = this.Model;
            this.GalleryViewCommand = new Command<Archive<CachedImage>>(ShowGallery);
            InitializeComponent();
        }
        private void ShowGallery(Archive<CachedImage> obj)
        {
            Shell.SetNavBarIsVisible(this, false);
            this.GalleryView.Show(this.Model.Photos.Select(x => x.Value), obj.Value);
        }
        protected override bool OnBackButtonPressed()
        {
            if (GalleryView.IsVisible)
            {
                Shell.SetNavBarIsVisible(this, true);
                this.GalleryView.Hide();
                return true;
            }
            return false;
        }
    }
}