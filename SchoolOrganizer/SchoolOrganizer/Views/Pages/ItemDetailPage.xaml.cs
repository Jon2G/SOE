using SchoolOrganizer.ViewModels.Pages;
using Xamarin.Forms;

namespace SchoolOrganizer.Views.Pages
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}