using AsyncAwaitBestPractices;
using SOE.API;
using SOE.Data;
using SOE.Models.Scheduler;
using SOE.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LinksPage : ContentPage
    {
        public static LinksPage Instance { get; private set; }
        public LinksPageViewModel Model { get; set; }
        public LinksPage(ClassSquare square)
        {
            Instance = this;
            this.Model = new LinksPageViewModel(square);
            this.BindingContext = this.Model;
            InitializeComponent();
        }
        private async Task SyncSubject()
        {
            await Task.Yield();
            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Actualizando información..."))
            {
                if (!await Model.ClassSquare.Subject.Sync(AppData.Instance, new SyncService()))
                {
                    await this.Model.Init(false);
                    return;
                }
            }
            this.Model.Init().SafeFireAndForget();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Model.ClassSquare.Subject.IsOffline)
            {
                SyncSubject().SafeFireAndForget(); 
                return;
            }
            this.Model.Init().SafeFireAndForget();
        }
    }
}