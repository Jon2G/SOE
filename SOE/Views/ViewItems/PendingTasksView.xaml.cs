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
    }
}