using SOE.ViewModels.Pages.Login;
using Xamarin.Forms.Xaml;

namespace SOE.Views.PopUps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SAESPrivacyAlert
    {
        public SAESPrivacyAlertViewModel Model { get; }
        public SAESPrivacyAlert()
        {
            this.Model = new SAESPrivacyAlertViewModel(this);
            this.BindingContext = Model;
            this.LockModal();
            InitializeComponent();
        }
    }
}