using SchoolOrganizer.ViewModels.Pages;
using SkiaSharp.Views.Forms;
using System;
using SchoolOrganizer.Models.SkiaSharp;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolOrganizer.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        readonly HighlightForm _highlightForm;
        public RegisterPage()
        {
            InitializeComponent();
            var settings = new HighlightSettings()
            {
                StrokeWidth = 6,
                StrokeStartColor = (Color)Application.Current.Resources["secondaryColor"],
                StrokeEndColor = (Color)Application.Current.Resources["primaryLightColor"],//Color.FromHex("#12396F"),
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

        protected override void OnAppearing()
        {
            
            base.OnAppearing();
            Usuario.Focus();

        }
    }
}