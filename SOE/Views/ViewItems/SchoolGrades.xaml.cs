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

        public SchoolGrades()
        {
            InitializeComponent();
            if (!AppData.Instance.User.HasSubjects)
            {
                this.Content = new NoInscriptionView();
                return;
            }
        }
        public override void OnAppearing()
        {
            this.Model.GetGrades();
        }
    }
}