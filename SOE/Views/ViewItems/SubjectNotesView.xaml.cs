using System.Threading.Tasks;
using SOEWeb.Shared;
using SOE.Data;
using SOE.Fonts;
using SOE.ViewModels.ViewItems;
using Xamarin.Forms.Xaml;

namespace SOE.Views.ViewItems
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubjectNotesView
    {
        public SubjectNotesViewModel Model { get; set; }
        public SubjectNotesView(Subject Subject)
        {
            this.Model = new SubjectNotesViewModel(Subject);
            this.BindingContext = this.Model;
            InitializeComponent();

        }
        public Task Init() => SoeWebView.GoToSubjectNotesPage(Model.Subject, AppData.Instance.User);

    }
}