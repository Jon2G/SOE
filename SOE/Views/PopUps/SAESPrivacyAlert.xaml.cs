using Xamarin.Forms.Xaml;

namespace SOE.Views.PopUps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SAESPrivacyAlert
    {
        public SAESPrivacyAlert()
        {
            this.LockModal();
            InitializeComponent();
        }
    }
}