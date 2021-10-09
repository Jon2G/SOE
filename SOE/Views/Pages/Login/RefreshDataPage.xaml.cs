using AsyncAwaitBestPractices;
using Kit;
using System;
using System.Threading.Tasks;
using SOE.Data;
using SOE.Models.SkiaSharp;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RefreshDataPage
    {
        public string Text { get; }
        public bool FirstTime { get; set; }
        public RefreshDataPage(bool FirstTime, bool? isOnline = null)
        {
            this.FirstTime = FirstTime;
            if (FirstTime)
            {
                Text = "Preparando todo para su primer uso";
            }
            else
            {
                Text = "Actualizando sus datos";
            }
            InitializeComponent();
            this.Model.IsOnline = isOnline;
            AppData.Instance.SAES = this.SAES;
            AppData.Instance.SAES.ShowLoading = false;
            InitAnimation();
            if (Tools.Debugging)
                this.ContentView.Opacity = 0.5;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            AppData.CreateDatabase();
            if (FirstTime)
                Model.GetUserData().SafeFireAndForget();
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