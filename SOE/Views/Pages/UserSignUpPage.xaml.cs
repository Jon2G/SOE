using System;
using System.Threading.Tasks;
using APIModels;
using AsyncAwaitBestPractices;
using SkiaSharp.Views.Forms;
using SOE.Data;
using SOE.Models.Data;
using SOE.Models.SkiaSharp;
using SOE.Saes;
using SOE.ViewModels.Pages;
using SOE.Views.PopUps;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserSignUpPage
    {
        public UserSignUpPageViewModel Model { get; set; }
        private HighlightForm _highlightForm;

        public UserSignUpPage()
        {
            InitializeComponent();
            this.Model = new UserSignUpPageViewModel(this.FirstForm, this.SecondForm);
            this.BindingContext = this.Model;
            AppData.Instance.SAES = this.SAES;
        }
        void EntryFocused(object sender, FocusEventArgs e)
        {
            _highlightForm?.HighlightElement((View)sender, _skCanvasView, _formLayout);
        }
        void SkCanvasViewPaintSurfaceRequested(object sender, SKPaintSurfaceEventArgs e)
        {
            _highlightForm?.Draw(_skCanvasView, e.Surface.Canvas);
        }
        void SkCanvasViewSizeChanged(object sender, EventArgs e)
        {
            _highlightForm?.Invalidate(_skCanvasView, _formLayout);
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (AppData.Instance.User.School is null)
            {
                App.Current.MainPage.Navigation.
                    PushModalAsync(new SchoolSelector())
                    .SafeFireAndForget();
                return;
            }
            SAESPrivacyAlert alert = new SAESPrivacyAlert();
            await alert.ShowDialog();
            await AppData.Instance.SAES.GoHome();
            if (await SAES.IsLoggedIn())
            {
                if (string.IsNullOrEmpty(AppData.Instance.User.Password))
                {
                    await AppData.Instance.SAES.LogOut();
                    OnAppearing();
                    return;
                }
                await SAES.GetCurrentUser();
                this.Model.OnValidationSuccessCommand.Execute(null);
            }
            else
            {
                Usuario.Focus();
                this.Model.RefreshCaptcha();
            }
        }
    }
}