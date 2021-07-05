using AsyncAwaitBestPractices;
using Kit.Forms.Pages;
using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Enums;
using Rg.Plugins.Popup.Services;
using SOE.Models.Scheduler;
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
    public partial class MenuHorarioPopUp 
    {
        public MenuHorarioPopUpViewModel Model { get; set; }
        public MenuHorarioPopUp(ClassSquare square)
        {
            this.Model = new MenuHorarioPopUpViewModel(this,square);
            this.BindingContext = Model;
            InitializeComponent();
        }

        
    }
}