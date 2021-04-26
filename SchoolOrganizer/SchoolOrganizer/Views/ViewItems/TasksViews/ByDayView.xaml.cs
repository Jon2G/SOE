using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolOrganizer.Models.TaskFirst;
using SchoolOrganizer.ViewModels.Pages;
using SchoolOrganizer.ViewModels.ViewItems;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace SchoolOrganizer.Views.ViewItems.TasksViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ByDayView : ContentView
    {
        public ByDayGroup Model => BindingContext as ByDayGroup;

        public TaskFirstViewModel Group => this.Parent?.BindingContext as TaskFirstViewModel;

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
            this.Model.RefreshCount();
        }
    }
}