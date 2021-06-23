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