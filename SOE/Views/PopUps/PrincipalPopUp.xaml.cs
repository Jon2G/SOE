using SOE.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.PopUps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class PrincipalPopUp 
    {
        public PrincipalPopUpViewModel Model { get; set; }
        public PrincipalPopUp()
        {
            this.Model = new PrincipalPopUpViewModel(this);
            this.BindingContext = Model;
            InitializeComponent();
        }
    }
}