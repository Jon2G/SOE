using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolOrganizer.ViewModels.Pages;
using SchoolOrganizer.Views.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolOrganizer.Saes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Test : ContentPage
    {
        private Saes Saes { get; set; }
        public Test()
        {
            InitializeComponent();
            Saes = new Saes();
            //this.Content = Saes.Browser;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

        }


    }
}