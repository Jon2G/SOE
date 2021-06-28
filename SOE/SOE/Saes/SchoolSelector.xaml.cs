using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolOrganizer.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolOrganizer.Saes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SchoolSelector : ContentPage
    {
        public SchoolSelectorViewModel Model { get; set; }
        public SchoolSelector(SchoolLevel schoolLevel)
        {
            this.Model = new SchoolSelectorViewModel(schoolLevel);
            this.BindingContext = this.Model;
            InitializeComponent();
            AppData.Instance.SAES = this.SAES;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.Model.Init();
        }
    }
}