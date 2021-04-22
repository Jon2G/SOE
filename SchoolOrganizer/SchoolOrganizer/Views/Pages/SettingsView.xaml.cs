
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolOrganizer.Models.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolOrganizer.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsView : ContentPage
    {

        public SettingsView()
        {
            InitializeComponent();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            this.Model.SaveCommand.Execute(null);
        }

        private void Switch_OnToggled(object sender, ToggledEventArgs e)
        {
           
        }
    }
}