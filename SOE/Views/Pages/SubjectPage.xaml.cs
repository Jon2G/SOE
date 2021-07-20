using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOEWeb.Shared;
using SOE.ViewModels.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubjectPage : ContentPage
    {
        public SubjectPageViewModel Model { get; set; }
        public SubjectPage(Subject Subject)
        {
            this.Model = new SubjectPageViewModel(Subject);
            this.BindingContext = this.Model;
            InitializeComponent();
        }
    }
}