using System.ComponentModel;
using Xamarin.Forms;
using OrganizadorEscolar.ViewModels;

namespace OrganizadorEscolar.Views
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