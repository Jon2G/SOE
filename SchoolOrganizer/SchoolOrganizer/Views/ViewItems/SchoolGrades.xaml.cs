using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolOrganizer.Data;
using SchoolOrganizer.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolOrganizer.Views.ViewItems
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SchoolGrades 
    {
        public SchoolGradesViewModel Model
        {
            get=>BindingContext as SchoolGradesViewModel;
            set => BindingContext = value;
        }
        public SchoolGrades()
        {
            InitializeComponent();
            Init();
        }
        private async void Init()
        {
            await Task.Yield();
            this.Model = new SchoolGradesViewModel();
            OnPropertyChanged(nameof(Model));
            OnPropertyChanged(nameof(BindingContext));

        }
    }
}