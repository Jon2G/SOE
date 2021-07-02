using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using SOE.Data;
using SOE.ViewModels.Pages;
using SOE.ViewModels.Pages.Login;
using SOE.ViewModels.ViewItems;
using Xamarin.Forms.Xaml;

namespace SOE.Views.PopUps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AskForCaptcha
    {
        public AskForCaptchaViewModel Model
        {
            get;
            set;
        }
        public AskForCaptcha(Func<AskForCaptcha, Task<bool>> OnSucceedAction)
        {
            this.Model = new AskForCaptchaViewModel(this,OnSucceedAction);
            this.BindingContext = this.Model;
            InitializeComponent();
            AppData.Instance.SAES = this.SAES;
            AppData.Instance.SAES.ShowLoading = true;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await AppData.Instance.SAES.GoHome();
            await Task.Delay(TimeSpan.FromSeconds(1));
            if (!await AppData.Instance.SAES.IsLoggedIn())
            {
                await AppData.Instance.SAES.GetCaptcha();
            }
            else
            {
                await this.Model.OnSucceedAction.Invoke(this);
            }
        }
    }
}