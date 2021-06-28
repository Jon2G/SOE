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
        }


    }
}