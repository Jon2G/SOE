using System;
using Xamarin.Forms;

namespace SchoolOrganizer.Views.Pages
{
    public partial class AboutPage : ContentPage
    {


        public AboutPage()
        {
            InitializeComponent();
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}