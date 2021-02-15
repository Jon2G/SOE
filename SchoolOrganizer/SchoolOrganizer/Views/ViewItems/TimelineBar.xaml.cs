using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolOrganizer.Views.ViewItems
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimelineBar : ContentView
    {
        public TimelineBar()
        {
            InitializeComponent();
        }

        private void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            
        }
    }
}