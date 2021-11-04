using SOE.Data;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WalkthroughPage
    {
        public WalkthroughPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (AppData.Instance.User.Settings is null)
                AppData.Instance.User.GetSettings();
        }
    }
}