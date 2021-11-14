using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsView 
    {

        public SettingsView()
        {
            InitializeComponent();
        }
        public override void OnSleep()
        {
            base.OnSleep();
            this.Model.SaveCommand.Execute(null);
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            this.Model.SaveCommand.Execute(null);
        }
    }
}