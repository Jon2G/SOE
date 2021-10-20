using System.Linq;
using SOE.Models.TodoModels;
using SOE.ViewModels.ViewItems;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.ViewItems.TasksViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ByDayView : ContentView
    {
        public ByDayGroup Model => BindingContext as ByDayGroup;

        public PendingTasksViewModel Group => this.Parent?.BindingContext as PendingTasksViewModel;

        public ByDayView()
        {
            InitializeComponent();
        }


        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            OnPropertyChanged(nameof(Model));
            if (Model != null)
            {
                Model.View = this;
            }
        }
        public void Resize()
        {
            if (!Model.SubjectGroups.Any())
            {
                Group?.DayGroups.Remove(this.Model);
                //la automatación
            }
            this.Model?.RefreshCount(); 
        }
    }
}