using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SchoolOrganizer.Models.TaskFirst;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolOrganizer.Views.ViewItems.TasksViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BySubjectGroupView : ContentView
    {
        public BySubjectGroup Model => BindingContext as BySubjectGroup;
        private ByDayGroup _Group;
        public ByDayGroup Group
        {
            get => _Group;
            set { _Group = value; OnPropertyChanged(); }
        }
        public BySubjectGroupView()
        {
            InitializeComponent();
        }
        protected override void OnParentSet()
        {
            base.OnParentSet();
            if (Parent != null)
                this.Group = this.Parent.BindingContext as ByDayGroup;
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
            Expander.ForceUpdateSize();
            if (!Model.ToDoS.Any())
            {
                Group.SubjectGroups.Remove(this.Model);
                //la automatación
            }
        }
    }
}