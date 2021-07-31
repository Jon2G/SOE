using SOE.ViewModels.ViewItems;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReminderPage 
    {
        public ReminderPageVIewModel Model { get; set; }
        public ReminderPage()
        {
            this.Model = new ReminderPageVIewModel(this);
            this.BindingContext = Model;
            InitializeComponent();
        }
    }
}