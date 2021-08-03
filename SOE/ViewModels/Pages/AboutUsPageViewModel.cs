using AsyncAwaitBestPractices;
using Kit;
using Kit.Model;
using SOE.Views.Pages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SOE.ViewModels.Pages
{
    public class AboutUsPageViewModel : ModelBase
    {
        public ICommand BuyMeACoffeCommand { get; }
        public ICommand GMailCommand { get; }
        public ICommand XamarinCommand { get; }
        public ICommand GitHubCommand { get; }
        public ICommand ReportBugCommand { get; }
        public ICommand RequestFeatureCommand { get; }
        public ICommand TrelloCommand { get; }
        public ICommand PrivacityCommand { get; }
        public AboutUsPageViewModel()
        {
            this.BuyMeACoffeCommand = new Command(BuyMeACoffe);
            this.XamarinCommand = new Command(Xamarin);
            this.GitHubCommand = new Command(GitHub);
            this.ReportBugCommand = new Command(ReportBug);
            this.RequestFeatureCommand = new Command(RequestFeature);
            this.GMailCommand = new Command(GMail);
            this.TrelloCommand = new Command(Trello);
            this.PrivacityCommand = new Command(Privacity);
        }



        private async void GMail()
        {
            try
            {
                var message = new EmailMessage
                {
                    Subject = "SOE App",
                    Body = "Solicitud de soporte",
                    To = new List<string>() { "soeapp.soporte@gmail.com" },
                    //Cc = ccRecipients,
                    //Bcc = bccRecipients
                };
                await Email.ComposeAsync(message);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, nameof(GMail));
            }

        }
        private void Trello() => OpenBrowser("https://trello.com/b/D5bXns3x/soe-app");
        private void GitHub() => OpenBrowser("https://github.com/Jon2G/SOE");




        private void ReportBug() => OpenBrowser("https://github.com/Jon2G/SOE/issues/new?assignees=&labels=&template=reporte-de-bug.md&title=");
        private void RequestFeature() => OpenBrowser("https://github.com/Jon2G/SOE/issues/new?assignees=&labels=&template=solicitud-de-funci-n-adicional.md&title=");

        private void Xamarin() => OpenBrowser("https://dotnet.microsoft.com/learn/xamarin/what-is-xamarin");

        private void BuyMeACoffe() => OpenBrowser("https://www.buymeacoffee.com/soeapp");

        private void Privacity(object obj)
        {
            Shell.Current.FlyoutIsPresented = false;
            Shell.Current.Navigation.PushAsync(new PrivacityPage()).SafeFireAndForget(); ;
        }

        private async void OpenBrowser(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, nameof(OpenBrowser));
            }
        }
    }
}
