using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolOrganizer.ViewModels.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolOrganizer.Views.PopUps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubjectPopUp 
    {
        public SubjectViewModel Modelo { get; set; }
        public SubjectPopUp()
        {
            this.Modelo=new SubjectViewModel(this); 
            InitializeComponent();
            this.BindingContext = this.Modelo;
        }


    }
}