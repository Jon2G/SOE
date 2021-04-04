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
        }

        internal async void OnAppearing()
        {
            this.Model = new SchoolGradesViewModel();
            if (await AppData.Instance.SAES.IsLoggedIn()) 
                this.Model.RefreshCommand.Execute(this);
        }

    }
}