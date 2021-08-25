using AsyncAwaitBestPractices;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

namespace SOE.Views.ViewItems
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PendingTasksView
    {
        public static PendingTasksView Instance { get; private set; }
        public override string Title => "TAREAS";

        public PendingTasksView()
        {
            Instance = this;
            InitializeComponent();
        }

        public override void OnAppearing()
        {
            if (this.Model.DayGroups.Any())
            {
                this.Model.DayGroups.First().ExpandAll(true);
            }
        }
        public Task Init => this.Model.Refresh();
    }
}