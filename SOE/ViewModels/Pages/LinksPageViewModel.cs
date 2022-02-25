using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using Kit;
using Kit.Model;
using Kit.Services.Web;
using SOE.API;
using SOE.Data;
using SOE.Models;
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
    public class LinksPageViewModel : ModelBase
    {
        public ObservableCollection<Link> Links { get; set; }
        public ClassSquare ClassSquare { get; }

        private ICommand _AddLinkCommand;
        public ICommand AddLinkCommand => this._AddLinkCommand ??= new Command(AddLink, () => !IsLoading && !IsOffline);

        private ICommand _OpenLinkCommand;
        public ICommand OpenLinkCommand => this._OpenLinkCommand ??= new Command<Link>(this.OpenLink);
        private ICommand _ReportCommand;
        public ICommand ReportCommand => _ReportCommand ??= new Command<Link>(ReportLinkPopUp.ShowPopUp);
        private ICommand _DeleteCommand;
        public ICommand DeleteCommand => _DeleteCommand ??= new Command<Link>(DeleteLinkPopUp.ShowPopUp);
        private bool _IsOffline;
        public bool IsOffline
        {
            get => _IsOffline;
            set
            {
                _IsOffline = value;
                Raise(() => IsOffline);
            }
        }
        private bool _IsLoading;
        public bool IsLoading
        {
            get => _IsLoading;
            set
            {
                _IsLoading = value;
                Raise(() => IsLoading);
            }
        }
        public ICommand RetryCommand { get; }
        private void OpenLink(Link link)
        {
            Browser.OpenAsync(link.Url, BrowserLaunchMode.SystemPreferred).SafeFireAndForget();
        }

        private void AddLink()
        {
            if (!AddLinkCommand.CanExecute(null))
            {
                return;
            }
            AddLinkPopUp popup = new(this.ClassSquare);
            popup.Show().SafeFireAndForget();
        }

        public LinksPageViewModel(ClassSquare square)
        {
            this.Links = new ObservableCollection<Link>();
            this.ClassSquare = square;
            this.RetryCommand = new AsyncCommand(() => Init(!IsOffline));
        }

        public async Task Init(bool Online = true)
        {
            IsLoading = true;
            IsOffline = false;
            await Task.Delay(100);
            if (!Online)
            {
                IsOffline = true;
                IsLoading = false;
                return;
            }
            Links.Clear();
            //if (ClassSquare.Subject.IsOffline)
            //{
            //    if (!await ClassSquare.Subject.Sync(AppData.Instance, new SyncService()))
            //    {
            //        IsOffline = true;
            //        IsLoading = false;
            //        return;
            //    }
            //}

            //var response= await APIService.Current.GetLinks(ClassSquare.Subject);
            //switch (response.ResponseResult)
            //{
            //    case APIResponseResult.OK:
            //        IsOffline = false;
            //        this.Links.AddRange(response.Extra);
            //        break;
            //    case APIResponseResult.INTERNAL_ERROR:
            //        this.IsOffline = true;
            //        break;
            //    default:
            //        this.IsOffline = true;
            //        Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Alerta", response.Message).SafeFireAndForget();
            //        break;
            //}
            this.IsLoading = false;
        }
    }

}
