using System;
using System.ComponentModel;
using System.Threading.Tasks;
using SOE.ViewModels.ViewItems;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.ViewItems.TasksViews
{
    [DesignTimeVisible(false)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainTaskView 
    {
        public static MainTaskView Instance { get; private set; }

        public MainTaskView()
        {
            Instance = this;
            InitializeComponent();

        }
        public void OnAppearing()
        {
            this.Model.OnAppearing();
        }
    }
}