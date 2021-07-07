using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using SOE.Data;
using SOE.Fonts;
using SOE.ViewModels;
using Xamarin.Forms.Xaml;

namespace SOE.Views.ViewItems
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SchoolGrades
    {
        public override string Title => "Calificaciones";
        public override string Icon => FontelloIcons.Cal;
        //public static SchoolGrades Instance { get; private set; }
        public SchoolGradesViewModel Model
        {
            get;
            set;
        }
        public SchoolGrades()
        {
            //Instance = this;
            this.Model = new SchoolGradesViewModel();
            this.BindingContext = this.Model;
            InitializeComponent();
            this.ToolbarItem.Command = this.Model.RefreshCommand;
            if (!AppData.Instance.User.HasSubjects)
            {
                this.Content = new NoInscriptionView();
                return;
            }
            Init().SafeFireAndForget();
        }
        private async Task Init()
        {
            await Task.Yield();
            this.Model.GetGrades();
        }
    }
}