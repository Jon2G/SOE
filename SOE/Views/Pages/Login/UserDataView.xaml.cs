
using SOE.ViewModels.Pages.Login;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserDataView
    {
        public UserDataView(UserSignUpPageViewModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}