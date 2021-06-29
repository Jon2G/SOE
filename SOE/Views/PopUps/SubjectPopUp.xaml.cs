using SOE.ViewModels.Pages;
using Xamarin.Forms.Xaml;

namespace SOE.Views.PopUps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubjectPopUp 
    {
        public SubjectViewModel Modelo { get; set; }
        public SubjectPopUp()
        {
            this.Modelo=new SubjectViewModel(this); 
            InitializeComponent();
            this.BindingContext = this.Modelo;
            App.Current.RequestedThemeChanged += Current_RequestedThemeChanged;
        }

        private void Current_RequestedThemeChanged(object sender, Xamarin.Forms.AppThemeChangedEventArgs e)
        {
            this.Modelo.Refresh();
        }
    }
}