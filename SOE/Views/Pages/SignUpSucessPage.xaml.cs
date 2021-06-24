using System;
using System.Linq;
using System.Threading.Tasks;
using APIModels;
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
    public partial class SignUpSucessPage
    {
        public SignUpSucessPage()
        {
            InitializeComponent();
            AppData.Instance.SAES = this.SAES;
            AppData.Instance.SAES.ShowLoading = false;
            InitAnimation();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Model.GetUserData();
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