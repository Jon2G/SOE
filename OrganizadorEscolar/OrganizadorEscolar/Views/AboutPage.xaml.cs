using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OrganizadorEscolar.Views
{
    public partial class AboutPage : ContentPage
    {
        bool _isFlyoutOpen = false;
        private const uint _flyoutSpeed = 200;
        private const uint Scale =10;
        private const uint HalfScale =Scale/2;

        public AboutPage()
        {
            InitializeComponent();
        }
        void OnToggleMenuOpen(object sender, EventArgs e)
        {
            if (_isFlyoutOpen)
            {
                return;
            }
            _isFlyoutOpen = true;
            ToggleFlyout();
        }

        void OnToggleMenuClose(object sender, EventArgs e)
        {
            if (!_isFlyoutOpen)
            {
                return;
            }
            _isFlyoutOpen = false;
            ToggleFlyout();
        }
        void OnToggleMenu(object sender, EventArgs e)
        {
            _isFlyoutOpen = !_isFlyoutOpen;
            ToggleFlyout();
        }

        void FlyoutClose(object sender, SwipedEventArgs e)
        {
            if (_isFlyoutOpen)
                ToggleFlyout();
        }

        void FlyoutOpen(object sender, SwipedEventArgs e)
        {
            if (!_isFlyoutOpen)
                ToggleFlyout();
        }

        void ToggleFlyout()
        {
            if (!_isFlyoutOpen)
            {
                Timeline.ScaleXTo(1, _flyoutSpeed);
                Timeline.TranslateTo(0, Flyout.TranslationY, _flyoutSpeed);
            }
            else
            {
                Timeline.ScaleXTo(Scale, _flyoutSpeed);
                Timeline.TranslateTo((Timeline.Width * -HalfScale) + Timeline.Width/2, Timeline.TranslationY, _flyoutSpeed);
            }

        }
    }
}