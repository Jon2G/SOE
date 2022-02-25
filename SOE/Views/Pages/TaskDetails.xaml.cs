using SOE.Data.Archives;
using SOE.Models.TodoModels;
using SOE.ViewModels.Pages;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Skip)]
    public partial class TaskDetails : ContentPage
    {
        public TaskDetailsViewModel Model { get; set; }
        private ICommand _GalleryViewCommand;
        public ICommand GalleryViewCommand => this._GalleryViewCommand ??= new Command<PhotoArchive>(ShowGallery);

        public TaskDetails(ToDo todo)
        {
            this.Model = new TaskDetailsViewModel(todo);
            BindingContext = this.Model;
            InitializeComponent();
        }
        private void ShowGallery(PhotoArchive obj)
        {
            this.GalleryView.Show(this.Model.Photos.Select(x => x.Value), obj.Value);
        }

        protected override bool OnBackButtonPressed()
        {
            if (GalleryView.IsVisible)
            {
                this.GalleryView.Hide();
                return true;
            }
            return false;
        }
    }
}