using AsyncAwaitBestPractices;
using Kit.Services.Interfaces;
using System;
using System.Threading.Tasks;
using SOE.Data;
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
        public AskForCaptcha(Func<ICrossWindow, Task<bool>> OnSucceedAction)
        {
            this.Model = new AskForCaptchaViewModel(this, OnSucceedAction);
            this.BindingContext = this.Model;
            InitializeComponent();
            AppData.Instance.SAES = this.SAES;
            AppData.Instance.SAES.ShowLoading = true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            GetCaptcha().SafeFireAndForget();
        }

        private async Task GetCaptcha()
        {
            await Task.Yield();
            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Cargando..."))
            {
                await this.GetCaptchaAsync();
            }
        }
        private async Task GetCaptchaAsync()
        {
            await Task.Yield();
            await AppData.Instance.SAES.GoHome();
            await Task.Delay(TimeSpan.FromSeconds(1));
            if (!await AppData.Instance.SAES.IsLoggedIn())
            {
                Model.CaptchaImg = await AppData.Instance.SAES.GetCaptcha();
            }
            else
            {
                await this.Model.OnSucceedAction.Invoke(this);
            }
        }
    }
}