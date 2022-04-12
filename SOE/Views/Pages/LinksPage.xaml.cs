using AsyncAwaitBestPractices;
using SOE.Models.Scheduler;
using SOE.ViewModels.Pages;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LinksPage : ContentPage
    {
        public static LinksPage Instance { get; private set; }
        public LinksPageViewModel Model { get; set; }
        public LinksPage(ClassSquare square)
        {
            Instance = this;
            this.Model = new LinksPageViewModel(square);
            this.BindingContext = this.Model;
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.Model.Init().SafeFireAndForget();
        }
    }
}