using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using SchoolOrganizer.ViewModels.Pages;
using SkiaSharp.Views.Forms;
using System;
using SchoolOrganizer.Models.SkiaSharp;
using SchoolOrganizer.Saes;
using Xamarin.Forms;
using System.Threading.Tasks;
using SchoolOrganizer.Data;
using SchoolOrganizer.Models.Data;
using Xamarin.Forms.Xaml;

namespace SchoolOrganizer.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private HighlightForm _highlightForm;

        public LoginPage(School School)
        {
            InitializeComponent();
            AppData.Instance.SAES = this.SAES;
            AppData.Instance.SAES.School = School;
            InitAnimation();
        }
        public LoginPage()
        {
            InitializeComponent();
            AppData.Instance.SAES =this.SAES;
            InitAnimation();
        }

        private void InitAnimation()
        {
            Task.Run(AnimateBorder);
            var settings = new HighlightSettings()
            {
                StrokeWidth = 6,
                StrokeStartColor = (Color)Application.Current.Resources["secondaryColor"],
                StrokeEndColor = (Color)Application.Current.Resources["primaryLightColor"],
                AnimationDuration = TimeSpan.FromMilliseconds(900),
                AnimationEasing = Easing.CubicInOut,
            };
            _highlightForm = new HighlightForm(settings);
        }

        void EntryFocused(object sender, FocusEventArgs e)
        {
            _highlightForm.HighlightElement((View)sender, _skCanvasView, _formLayout);
        }

        void SkCanvasViewPaintSurfaceRequested(object sender, SKPaintSurfaceEventArgs e)
        {
            _highlightForm.Draw(_skCanvasView, e.Surface.Canvas);
        }

        void SkCanvasViewSizeChanged(object sender, EventArgs e)
        {
            _highlightForm.Invalidate(_skCanvasView, _formLayout);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (!AppData.Instance.SAES.School.IsSchoolSelected)
            {
                await Navigation.PushModalAsync(new SchoolLevelSelector());
                return;
            }
            await SAES.GoTo(SAES.School.HomePage);
            if (await SAES.IsLoggedIn())
            {
                string boleta = await SAES.GetCurrentUser();
                AppData.Instance.User = AppData.Instance.LiteConnection.Get<User>(boleta);
                if (AppData.Instance.User is not null)
                {
                    this.Model.OnLoginSuccess.Execute(this.SAES);
                }
                else
                {
                    await SAES.LogOut();
                }
            }
            Usuario.Focus();
            this.Model.CaptchaImg = await this.SAES.GetCaptcha();

        }
        private async void AnimateBorder()
        {
            Action<double> tealMovement = tInput => tealGrad.Offset = (float)tInput;
            Action<double> orangeMovement = oInput => orangeGrad.Offset = (float)oInput;

            while (true)
            {
                mainRect.Animate(name: "forward", callback: tealMovement, start: 0, end: 1, length: 1000, easing: Easing.SinIn);
                await Task.Delay(1000);
                mainRect.Animate(name: "backward", callback: tealMovement, start: 1, end: 0, length: 1000, easing: Easing.SinIn);
                await Task.Delay(1000);

                mainRect.Animate(name: "forward2", callback: orangeMovement, start: 1, end: 0, length: 1000, easing: Easing.SinIn);
                await Task.Delay(1000);
                mainRect.Animate(name: "backward2", callback: orangeMovement, start: 0, end: 1, length: 1000, easing: Easing.SinIn);
                await Task.Delay(1000);
            }
        }
    }
}