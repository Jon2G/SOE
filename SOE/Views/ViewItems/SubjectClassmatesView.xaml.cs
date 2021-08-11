using SOEWeb.Shared;
using SOE.Fonts;
using SOE.ViewModels.ViewItems;
using Xamarin.Forms.Xaml;

namespace SOE.Views.ViewItems
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubjectClassmatesView
    {
        public override string Title => "COMPAÑEROS";
        public SubjectClassmatesViewModel Model { get; set; }
        public SubjectClassmatesView(Subject subject)
        {
            this.Model = new SubjectClassmatesViewModel(subject);
            this.BindingContext = this.Model;
            InitializeComponent();
        }
    }
}