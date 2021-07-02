using System.Threading.Tasks;
using SOE.Data;
using SOE.ViewModels;
using Xamarin.Forms.Xaml;

namespace SOE.Views.ViewItems
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SchoolGrades 
    {
        public SchoolGradesViewModel Model
        {
            get=>BindingContext as SchoolGradesViewModel;
            set => BindingContext = value;
        }
        public SchoolGrades()
        {
            InitializeComponent();
            if (!AppData.Instance.User.HasSubjects)
            {
                this.Content = new NoInscriptionView();
                return;
            }
            Init();
        }
        private async void Init()
        {
            await Task.Yield();
            this.Model = new SchoolGradesViewModel();
            OnPropertyChanged(nameof(Model));
            OnPropertyChanged(nameof(BindingContext));

        }
    }
}