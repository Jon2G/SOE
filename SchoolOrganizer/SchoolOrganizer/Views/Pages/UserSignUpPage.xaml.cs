using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using SchoolOrganizer.ViewModels.Pages;
using SkiaSharp.Views.Forms;
using System;
using SchoolOrganizer.Models.SkiaSharp;
using SchoolOrganizer.Saes;
using Xamarin.Forms;
using System.Threading.Tasks;
using APIModels;
using SchoolOrganizer.Data;
using SchoolOrganizer.Models.Data;
using SchoolOrganizer.Views.PopUps;
using Xamarin.Forms.Xaml;

namespace SchoolOrganizer.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserSignUpPage
    {
        public UserSignUpPageViewModel Model { get; set; }
        private HighlightForm _highlightForm;

        public UserSignUpPage(School School, User User)
        {
            InitializeComponent();
            this.Model = new UserSignUpPageViewModel(School, User,this.FirstForm, this.SecondForm);
            this.BindingContext = this.Model;
            AppData.Instance.SAES = this.SAES;
            AppData.Instance.User.School = School;
            InitAnimation();
        }

        private void InitAnimation()
        {
            Task.Run(AnimateBorder);
            var settings = new HighlightSettings()
            {
                StrokeWidth = 6,
                StrokeStartColor = (Color)Application.Current.Resources["SecondaryColor"],
                StrokeEndColor = (Color)Application.Current.Resources["PrimaryLightColor"],
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
            SAESPrivacyAlert alert = new SAESPrivacyAlert();
            await alert.ShowDialog();
            AppData.Instance.SAES.School = this.Model.School;
            await AppData.Instance.SAES.GoTo(this.Model.School.HomePage);
            if (await SAES.IsLoggedIn())
            {
                string boleta = await SAES.GetCurrentUser();
                AppData.Instance.User = AppData.Instance.LiteConnection.Get<User>(boleta);
                if (AppData.Instance.User is not null)
                {
                    this.Model.OnValidationSuccessCommand.Execute(null);
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