using SOE.ViewModels.Pages.Login;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ModeSelectorView : ContentView
    {
        private readonly UserSignUpPageViewModel UserSignUpPageViewModel;
        public ModeSelectorView(UserSignUpPageViewModel userSignUpPageViewModel)
        {
            this.UserSignUpPageViewModel = userSignUpPageViewModel;
            this.BindingContext = this.UserSignUpPageViewModel;
            InitializeComponent();
        }
    }
}