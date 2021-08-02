using AsyncAwaitBestPractices;
using Kit;
using Kit.Model;
using SOE.Models.Scheduler;
using SOE.Services;
using SOE.Views.PopUps;
using SOEWeb.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SOE.ViewModels.Pages
{
    public class LinksPageViewModel:ModelBase
    {
        public ObservableCollection<Link> Links { get; set; }
        public ClassSquare ClassSquare { get; }

        private ICommand _AddLinkCommand;
        public ICommand AddLinkCommand=> _AddLinkCommand ??= new Command(AddLink);

        private ICommand _OpenLinkCommand;
        public ICommand OpenLinkCommand => _OpenLinkCommand ??= new Command<Link>(OpenLink);

        private void OpenLink(Link link)
        {
            Browser.OpenAsync(link.Url, BrowserLaunchMode.External).SafeFireAndForget();
        }

        private void AddLink()
        {
            var popup = new AddLinkPopUp(ClassSquare);
            popup.Show().SafeFireAndForget();
        }

        public LinksPageViewModel(ClassSquare square)
        {
            this.Links = new ObservableCollection<Link>();
            this.ClassSquare = square;
        }
        public async Task Init()
        {
            Links.Clear();
            Links.AddRange(await LinkService.GetLinks(ClassSquare.Subject));
        }
    }

}
