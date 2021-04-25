using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json.Serialization;
using SchoolOrganizer.Data.Images;
using SchoolOrganizer.Models.TaskFirst;
using SchoolOrganizer.ViewModels.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolOrganizer.Views.Pages
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
            this.GalleryViewCommand = new Command<Archive<FileImageSource>>(ShowGallery);
            InitializeComponent();
        }
        private void ShowGallery(Archive<FileImageSource> obj)
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