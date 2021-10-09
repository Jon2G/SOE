using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SOEWeb.Shared;
using AsyncAwaitBestPractices;
using Kit;
using Kit.Model;
using Kit.Services.Web;
using SOE.API;
using SOE.Models.Scheduler;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace SOE.ViewModels.ViewItems
{
    public class SubjectClassmatesViewModel:ModelBase
    {
        public ObservableCollection<Classmate> Classmates { get; set; }
        public Subject Subject { get; set; }

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

        public SubjectClassmatesViewModel(Subject subject)
        {
            this.Subject = subject;
            Classmates = new ObservableCollection<Classmate>();
            this.RetryCommand = new AsyncCommand(this.Load);
            Load().SafeFireAndForget();
        }

        private async Task Load()
        {
            IsLoading = true;
            await Task.Delay(100);
            var response
                = await APIService.GetClassmates(this.Subject);
            switch (response.ResponseResult)
            {
                case APIResponseResult.OK:
                    this.Classmates.AddRange(response.Extra);
                    break;
                case APIResponseResult.INTERNAL_ERROR:
                    this.IsOffline = true;
                    break;
                default:
                    this.IsOffline = true;
                    Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Alerta", response.Message).SafeFireAndForget();
                    break;
            }
            IsLoading = false;
        }
    }
}
