using SOE.ViewModels.Pages.Login;
using System;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FreeModeStartupView
    {
        public FreeModeStartupView(FreeModePageViewModel model)
        {
            this.BindingContext = model;
            InitializeComponent();
            Task.Delay(TimeSpan.FromSeconds(0.5)).ContinueWith(t => this.Usuario.Focus());
        }
    }
}