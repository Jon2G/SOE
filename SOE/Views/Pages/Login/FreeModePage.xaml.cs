using SOE.ViewModels.Pages.Login;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FreeModePage
    {
        public FreeModePageViewModel Model { get; set; }
        public FreeModePage(FreeModePageViewModel freeModePageView)
        {
            this.Model = freeModePageView;
            this.BindingContext = this.Model;
            InitializeComponent();
        }
    }
}