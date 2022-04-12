using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using SOE.Models;
using SOE.Secrets;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BlogPage
    {
        public ViewModels.Pages.BlogPageViewModel Model { get; set; }

        private bool _IsOffline;
        public bool IsOffline
        {
            get => _IsOffline;
            set
            {
                _IsOffline = value;
                this.OnPropertyChanged();
            }
        }
        public ICommand RetryCommand { get; }
        private bool _IsLoading;

        public bool IsLoading
        {
            get => _IsLoading;
            set
            {
                _IsLoading = value;
                this.OnPropertyChanged();
            }
        }

        private readonly bool Online;
        public BlogPage(Subject Subject)
        {
            this.Model = new ViewModels.Pages.BlogPageViewModel(Subject);
            this.BindingContext = this.Model;
            this.RetryCommand = new AsyncCommand(this.Init);
            InitializeComponent();
            this.Init().SafeFireAndForget();
        }

        private async Task Init()
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
            this.SoeWebView.FailureCommand = new Kit.Extensions.Command<WebNavigationResult>((e) => IsOffline = true);
            if (Kit.Tools.Instance.RuntimePlatform != Kit.Enums.RuntimePlatform.iOS)
                await SoeWebView.GoTo(DotNetEnviroment.BaseUrl);
            //await SoeWebView.GoToSubjectNotesPage(Model.Subject, AppData.Instance.User);
            IsLoading = false;
        }

    }
}