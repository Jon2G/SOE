using SOE.Data;
using SOE.Enums;
using SOE.ViewModels.Pages.Login;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SaesLoginView : ContentView
    {
        public SaesLoginView(UserSignUpPageViewModel model)
        {
            AppData.Instance.User.Mode = UserMode.SAES;
            this.BindingContext = model;
            InitializeComponent();
            _ = Task.Delay(TimeSpan.FromSeconds(0.5)).ContinueWith(t =>
            Dispatcher.BeginInvokeOnMainThread(() =>
            {
                _ = this.Usuario.Focus();
            }));
        }
    }
}