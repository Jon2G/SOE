using SOE.Fonts;
using Xamarin.Forms.Xaml;

namespace SOE.Views.ViewItems
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainView
    {
        public override string Title => "Tareas";
        public override string Icon => FontelloIcons.Book;

        public MainView()
        {
            InitializeComponent();
            
        }
    }
}