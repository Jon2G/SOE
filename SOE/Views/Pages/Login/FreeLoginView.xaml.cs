using SOE.ViewModels.Pages.Login;
using System;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FreeLoginView
    {
        public FreeModePageViewModel Model { get; set; }
        public FreeLoginView(FreeModePageViewModel model)
        {
            Model = model;
            this.BindingContext = Model;
            InitializeComponent();
            Task.Delay(TimeSpan.FromSeconds(0.5)).ContinueWith(t => this.Usuario.Focus());
        }
    }
}