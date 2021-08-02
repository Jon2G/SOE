using SOE.Models.Scheduler;
using SOE.ViewModels.Pages;
using SOE.ViewModels.ViewItems;
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
    public partial class AddLinkPopUp 
    {
        public AddLinkPopUpViewModel Model { get; set; }
        public AddLinkPopUp(ClassSquare square)
        {
            this.Model = new AddLinkPopUpViewModel(square);
            this.BindingContext = this.Model;
            InitializeComponent();
        }
    }
}