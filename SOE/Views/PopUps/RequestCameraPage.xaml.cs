using Xamarin.Forms.Xaml;

namespace SOE.Views.PopUps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RequestCameraPage
    {
        public RequestCameraPage()
        {
            this.LockModal();
            InitializeComponent();
        }
    }
}