using AsyncAwaitBestPractices;
using SOE.Data;
using SOE.Enums;
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
                this.Content =
                    AppData.Instance.User.Mode == UserMode.SAES ?
                        new NoInscriptionView() :
                        new NoSubjectsView();
                return;
            }
        }
        public override void OnAppearing()
        {
            this.Model.GetGrades().SafeFireAndForget();
        }
    }
}