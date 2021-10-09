﻿using SOE.API;
using System.Threading.Tasks;
using SOEWeb.Shared;
using SOE.Data;
using SOE.Fonts;
using SOE.ViewModels.ViewItems;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.ViewItems
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubjectNotesView
    {
        public SubjectNotesViewModel Model { get; set; }
        public override string Title => "NOTAS";
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
        public SubjectNotesView(Subject Subject)
        {
            this.Model = new SubjectNotesViewModel(Subject);
            this.BindingContext = this.Model;
            this.RetryCommand = new AsyncCommand(this.Init);
            InitializeComponent();

        }

        public async Task Init()
        {
            IsLoading = true;
            IsOffline = false;
            await Task.Delay(100);
            this.SoeWebView.FailureCommand = new Kit.Extensions.Command<WebNavigationResult>((e) => IsOffline = true);
            await SoeWebView.GoTo(APIService.BaseUrl);
            await SoeWebView.GoToSubjectNotesPage(Model.Subject, AppData.Instance.User);
            IsLoading = false;
        }

    }
}