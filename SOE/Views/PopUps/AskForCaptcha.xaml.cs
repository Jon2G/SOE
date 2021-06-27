using System;
using SOE.ViewModels.Pages;
using SOE.ViewModels.Pages.Login;
using Xamarin.Forms.Xaml;

namespace SOE.Views.PopUps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AskForCaptcha
    {
        public LoginViewModel Model { get; set; }
        public AskForCaptcha(LoginViewModel Model)
        {
            this.Model = Model;
            this.BindingContext = this.Model;
            InitializeComponent();
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            //if (string.IsNullOrEmpty(this.Model.Captcha))
            //{
            //    return;
            //}
            await this.Close();
        }
    }
}