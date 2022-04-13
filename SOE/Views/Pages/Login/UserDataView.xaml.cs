
using SOE.ViewModels.Pages.Login;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserDataView : ContentView
    {
        public UserDataView(UserSignUpPageViewModel model)
        {
            this.BindingContext = model;
            InitializeComponent();
        }
    }
}