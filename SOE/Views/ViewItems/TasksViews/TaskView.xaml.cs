using SOE.Models.TodoModels;
using SOE.Models.TodoModels;
using Xamarin.Forms.Xaml;

namespace SOE.Views.ViewItems.TasksViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskView
    {
        public ToDo Model => BindingContext as ToDo;


        private BySubjectGroup _Group;
        public BySubjectGroup Group
        {
            get => _Group;
            set { _Group = value; OnPropertyChanged(); }
        }
        public TaskView()
        {
            InitializeComponent();
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
            if (Parent != null)
                this.Group = this.Parent.BindingContext as BySubjectGroup;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            OnPropertyChanged(nameof(Model));
        }
    }
}