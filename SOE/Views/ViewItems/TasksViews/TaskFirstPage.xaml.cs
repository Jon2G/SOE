using System.ComponentModel;
using SOE.ViewModels.ViewItems;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.ViewItems.TasksViews
{
    [DesignTimeVisible(false)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskFirstPage : ContentView
    {
        public static TaskFirstPage Instance { get; private set; }
       public TaskFirstViewModel Model { get; set; }
        public TaskFirstPage()
        {
            Instance = this;
            this.Model= new TaskFirstViewModel();
            BindingContext = this.Model;
            InitializeComponent();

        }


    }
}