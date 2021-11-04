using SOE.Models.Scheduler;
using SOE.ViewModels.Pages;
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