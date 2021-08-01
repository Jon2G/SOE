using SOE.Models;
using SOE.ViewModels.ViewItems;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReminderPage
    {
        public ReminderPageVIewModel Model { get; set; }

        public ReminderPage():this(new Reminder())
        {

        }
        public ReminderPage(Reminder reminder)
        {
            this.Model = new ReminderPageVIewModel(this) { PReminder = reminder };
            this.BindingContext = Model;
            InitializeComponent();
        }
    }
}