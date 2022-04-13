using SOE.ViewModels.Pages.Login;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FreeSignUpView : ContentView
    {
        public FreeModePageViewModel Model { get; set; }
        public FreeSignUpView(FreeModePageViewModel model)
        {
            Model = model;
            this.BindingContext = Model;
            InitializeComponent();
        }
    }
}