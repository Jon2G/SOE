using Xamarin.Forms.Xaml;

namespace SOE.Views.ViewItems
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PendingTasksView
    {
        public override string Title => "TAREAS";

        public PendingTasksView()
        {
            InitializeComponent();
        }
    }
}