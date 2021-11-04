using SOE.Fonts;
using Xamarin.Forms.Xaml;

namespace SOE.Views.ViewItems
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotificationView
    {
        public override string Title => "Notificaciones";

        public NotificationView()
        {
            InitializeComponent();
        }
    }
}