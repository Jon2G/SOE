using SchoolOrganizer.Models;
using SchoolOrganizer.ViewModels.Pages;
using Xamarin.Forms;

namespace SchoolOrganizer.Views.Pages
{
    public partial class NewItemPage : ContentPage
    {
        public Item Item { get; set; }

        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = new NewItemViewModel();
        }
    }
}